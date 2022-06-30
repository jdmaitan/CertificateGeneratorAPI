using CertificateGeneratorAPI.Data;
using CertificateGeneratorAPI.Models.InputModels;
using CertificateGeneratorAPI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateGeneratorAPI.Services.Interfaces
{
    public interface ICertificateData
    {
        #region Holder CRUD

        public Task<int> CreateHolder(HolderInput holderInput);

        public Task<List<Holder>> ReadHolders();

        public Task<List<Holder>> ReadHolders(HolderInputFilter holderInputFilter);

        public Task<Holder> ReadHolder(int id);

        public Task<Holder> ReadHolder(HolderInput holderInput);

        public Task UpdateHolder(int id, HolderInput holderInput);

        public Task DeleteHolders();

        public Task DeleteHolder(int id);

        #endregion

        #region Certificate CRUD

        public Task<CreatedCertificateViewModel> CreateCertificate(CertificateInput certificateInput);

        public Task<List<Certificate>> ReadCertificates();

        public Task<List<Certificate>> ReadCertificates(CertificateInputFilter certificateInputFilter);

        public Task<Certificate> ReadCertificate(int id);

        public Task<Certificate> ReadCertificate(Guid guid);

        public Task<Certificate> ReadCertificate(CertificateInput certificateInput);

        public Task<CertificatePDFViewModel> ReadCertificateViewModel(string guid);

        public Task UpdateCertificate(int id, CertificateInput certificateInput);

        public Task RenewCertificate(int id);

        public Task DeleteCertificates();

        public Task DeleteCertificate(int id);

        #endregion

        #region Certificate Type CRUD

        public Task<int> CreateCertificateType(CertificateTypeInput certificateTypeInput);

        public Task<List<CertificateType>> ReadCertificateTypes();

        public Task<List<CertificateType>> ReadCertificateTypes(CertificateTypeInputFilter certificateTypeInputFilter);

        public Task<CertificateType> ReadCertificateType(int id);

        public Task<CertificateType> ReadCertificateType(CertificateTypeInput certificateTypeInput);

        public Task UpdateCertificateType(int id, CertificateTypeInput certificateTypeInput);

        public Task DeleteCertificateTypes();

        public Task DeleteCertificateType(int id);

        #endregion
    }
}
