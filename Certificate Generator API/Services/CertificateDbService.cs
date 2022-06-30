using CertificateGeneratorAPI.Data;
using CertificateGeneratorAPI.Models.InputModels;
using CertificateGeneratorAPI.Models.ViewModels;
using CertificateGeneratorAPI.Services.Interfaces;
using CertificateGeneratorAPI.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateGeneratorAPI.Services
{
    public class CertificateDbService : ICertificateData
    {
        private readonly CertificateDbContext _context;
        private readonly CRUDSettings _crudSettings;

        public CertificateDbService(CertificateDbContext certificateDbContext,
                                    CRUDSettings crudSettings)
        {
            _context = certificateDbContext;
            _crudSettings = crudSettings;
        }

        #region Holder CRUD

        public async Task<int> CreateHolder(HolderInput holderInput)
        {
            Holder holder = holderInput.ToHolder();
            _context.Add(holder);
            await _context.SaveChangesAsync();
            return holder.ID;
        }

        public async Task<List<Holder>> ReadHolders()
        {
            return await _context.Holders
                        .Include(h => h.Certificates)
                        .ToListAsync();
        }

        public async Task<List<Holder>> ReadHolders(HolderInputFilter holderInputFilter)
        {
            return await _context.Holders
                        .Where(string.IsNullOrEmpty(holderInputFilter.BusinessName) ? h => true : h => h.BusinessName.Contains(holderInputFilter.BusinessName))
                        .Where(string.IsNullOrEmpty(holderInputFilter.RIF) ? h => true : h => h.RIF.Contains(holderInputFilter.RIF))
                        .Include(h => h.Certificates)
                        .Take( holderInputFilter.Limit <= 0 ? _crudSettings.MaxReadResults : (holderInputFilter.Limit))
                        .ToListAsync();
        }

        public async Task<Holder> ReadHolder(int id)
        {
            return await _context.Holders
                        .Where(h => h.ID == id)
                        .Include(h => h.Certificates)
                        .SingleOrDefaultAsync();
        }

        public async Task<Holder> ReadHolder(HolderInput holderInput)
        {
            return await _context.Holders.Where(h => h.RIF == holderInput.RIF.ToUpper().Replace(" ","").Replace("-",""))
                                         .SingleOrDefaultAsync();
        }

        public async Task UpdateHolder(int id, HolderInput holderInput)
        {
            Holder holder = await _context.Holders.FindAsync(id);

            if (holder == null)
            {
                throw new Exception($"Couldn't find holder with ID = {id}");
            }

            holder.BusinessName = holderInput.BusinessName;
            holder.RIF = holderInput.RIF;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHolders()
        {
            List<Holder> holders = await _context.Holders.ToListAsync();
            _context.Holders.RemoveRange(holders);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHolder(int id)
        {
            Holder holder = await ReadHolder(id);

            if (holder == null)
            {
                throw new Exception($"Couldn't find holder with ID = {id}");
            }

            _context.Remove(holder);

            await _context.SaveChangesAsync();
        }

        #endregion Holder CRUD

        #region Certificate CRUD

        public async Task<CreatedCertificateViewModel> CreateCertificate(CertificateInput certificateInput)
        {
            Holder holder = await _context.Holders.FindAsync(certificateInput.HolderID);

            if (holder == null)
            {
                throw new Exception($"Couldn't find holder with ID = {certificateInput.HolderID}");
            }

            CertificateType certificateType = await _context.CertificateTypes.FindAsync(certificateInput.TypeID);

            if (certificateType == null)
            {
                throw new Exception($"Couldn't find certificate type with ID = {certificateInput.TypeID}");
            }

            Guid GUID;
            Certificate foundCertificate;

            do
            {
                GUID = Guid.NewGuid();
                foundCertificate = await ReadCertificate(GUID);
            }
            while (foundCertificate != null);

            Certificate certificate = new Certificate
            {
                HolderID = holder.ID,
                TypeID = certificateType.ID,
                ExpeditionDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddMonths(certificateType.ValidityMonths),
                GUID = GUID
            };

            _context.Add(certificate);
            await _context.SaveChangesAsync();



            return new CreatedCertificateViewModel
            {
                ID = certificate.ID,
                GUID = certificate.GUID.ToString()
            };
        }

        public async Task<List<Certificate>> ReadCertificates()
        {
            return await _context.Certificates.ToListAsync();
        }

        public async Task<List<Certificate>> ReadCertificates(CertificateInputFilter certificateInputFilter)
        {
            return await _context.Certificates.Where(certificateInputFilter.HolderID == 0 ? c => true : c => c.HolderID == certificateInputFilter.HolderID)
                                              .Where(certificateInputFilter.TypeID == 0 ? c => true : c => c.TypeID == certificateInputFilter.TypeID)
                                              .Take( certificateInputFilter.Limit <= 0 ? _crudSettings.MaxReadResults : certificateInputFilter.Limit)
                                              .ToListAsync();
        }

        public async Task<Certificate> ReadCertificate(int id)
        {
            return await _context.Certificates
                        .Where(c => c.ID == id)
                        .SingleOrDefaultAsync();
        }

        public async Task<Certificate> ReadCertificate(Guid guid)
        {

            return await _context.Certificates
                        .Where(c => c.GUID == guid)
                        .SingleOrDefaultAsync();
        }

        public async Task<Certificate> ReadCertificate(CertificateInput certificateInput)
        {
            return await _context.Certificates.Where(c => c.TypeID == certificateInput.TypeID)
                                              .Where(c => c.HolderID == certificateInput.HolderID)
                                              .SingleOrDefaultAsync();
        }

        public async Task<CertificatePDFViewModel> ReadCertificateViewModel(string guid)
        {
            Certificate certificate = await ReadCertificate(Guid.Parse(guid));

            if (certificate == null)
            {
                throw new Exception($"Unable to find certificate with GUID {guid}");
            }

            Holder holder = await ReadHolder(certificate.HolderID);
            CertificateType certificateType = await ReadCertificateType(certificate.TypeID);

            return new CertificatePDFViewModel
            {
                BusinessName = holder.BusinessName,
                RIF = holder.RIF,
                Type = certificateType.Description,
                ExpeditionDate = certificate.ExpeditionDate,
                ExpirationDate = certificate.ExpirationDate,
                GUID = certificate.GUID.ToString()
            };
        }

        public async Task UpdateCertificate(int id, CertificateInput certificateInput)
        {
            Certificate certificate = await _context.Certificates.FindAsync(id);

            if (certificate == null)
            {
                throw new Exception($"Unable to find certificate with ID {id}");
            }

            certificateInput.UpdateCertificate(certificate);
            await _context.SaveChangesAsync();
        }

        public async Task RenewCertificate(int id)
        {
            Certificate certificate = await _context.Certificates.FindAsync(id);

            if (certificate == null)
            {
                throw new Exception($"Unable to find certificate with ID {id}");
            }

            CertificateType certificateType = await _context.CertificateTypes.FindAsync(certificate.TypeID);

            certificate.ExpeditionDate = DateTime.Now;
            certificate.ExpirationDate = DateTime.Now.AddMonths(certificateType.ValidityMonths);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCertificates()
        {
            List<Certificate> certificates = await _context.Certificates.ToListAsync();
            _context.Certificates.RemoveRange(certificates);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCertificate(int id)
        {
            Certificate certificate = await ReadCertificate(id);

            if (certificate == null)
            {
                throw new Exception($"Couldn't find certificate with ID = {id}");
            }

            _context.Remove(certificate);

            await _context.SaveChangesAsync();
        }


        #endregion Certificate CRUD

        #region CertificateType CRUD

        public async Task<int> CreateCertificateType(CertificateTypeInput certificateTypeInput)
        {
            CertificateType certificateType = certificateTypeInput.ToCertificateType();
            _context.Add(certificateType);
            await _context.SaveChangesAsync();
            return certificateType.ID;
        }

        public async Task<List<CertificateType>> ReadCertificateTypes()
        {
            return await _context.CertificateTypes.ToListAsync();
        }

        public async Task<List<CertificateType>> ReadCertificateTypes(CertificateTypeInputFilter certificateTypeInputFilter)
        {
            return await _context.CertificateTypes.Where(string.IsNullOrEmpty(certificateTypeInputFilter.Description) ? t => true : t => t.Description.Contains(certificateTypeInputFilter.Description))
                                                  .Where(certificateTypeInputFilter.ValidityMonths == 0 ? t => true : t => t.ValidityMonths == certificateTypeInputFilter.ValidityMonths)
                                                  .Take( certificateTypeInputFilter.Limit <= 0 ? _crudSettings.MaxReadResults : (certificateTypeInputFilter.Limit))
                                                  .ToListAsync();
        }

        public async Task<CertificateType> ReadCertificateType(int id)
        {
            return await _context.CertificateTypes
                        .Where(t => t.ID == id)
                        .SingleOrDefaultAsync();
        }

        public async Task<CertificateType> ReadCertificateType(CertificateTypeInput certificateTypeInput)
        {
            return await _context.CertificateTypes.Where(t => t.Description == certificateTypeInput.Description)
                                                  .SingleOrDefaultAsync();
        }

        public async Task UpdateCertificateType(int id, CertificateTypeInput certificateTypeInput)
        {
            CertificateType certificateType = await _context.CertificateTypes.FindAsync(id);

            if (certificateType == null)
            {
                throw new Exception($"Unable to find certificate type with ID {id}");
            }

            certificateTypeInput.UpdateCertificateType(certificateType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCertificateTypes()
        {
            List<CertificateType> certyficateTypes = await _context.CertificateTypes.ToListAsync();
            _context.CertificateTypes.RemoveRange(certyficateTypes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCertificateType(int id)
        {
            CertificateType certificateType = await ReadCertificateType(id);

            if (certificateType == null)
            {
                throw new Exception($"Couldn't find certificate type with ID = {id}");
            }

            _context.Remove(certificateType);

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}

