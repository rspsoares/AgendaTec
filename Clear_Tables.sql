DELETE FROM TSchedules
DELETE FROM TCGServices
DELETE FROM TCGCustomers WHERE IDCustomer <> 1
DELETE FROM TCGProfessionals
DELETE FROM AspNetUsers WHERE UserName <> 'rodrigo.soares@avalara.com' AND UserName <> 'bruno@bvf.com.br'