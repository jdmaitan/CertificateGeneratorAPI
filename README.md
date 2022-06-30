# CertificateGeneratorAPI
REST API used for handling certificate operations.

This API would be useful for companies/institutions/corporations that need a solution to give certificates or awards to customers, staff, students, etc.

Inside, there's a couple of examples of the types of certificates you could generate using the given template.

Bear in mind that this solution was developed for the Venezuelan market, so it uses the parameter "RIF" as the personal ID for any taxpayer of the country. You can change the logic of this ID for whichever is used in the target country.

Also, it is worth noting that all methods have a JWT authentication with the exception of "GetCertificatePDF" and "Authenticate".

The default example authentication data is set to:

- **Username: test@gmail.com**
- **Password: 1234**

If you'd like to test the methods without authentication, you can comment the [Authorize("Bearer")] attributes in the controllers.

Some examples of certificates GUID already saved in the database for testing are:

- 06026EF9-7EA8-4FFC-A7DD-12E1C0E5F89D
- 4562DCE3-CDA6-4A57-8A7A-24A84B6469FB
- 7AB71660-408D-478E-B79B-20FD6ED67B4B

Use these GUIDs with the GetCertificatePDF to obtain the example certificate PDFs.
