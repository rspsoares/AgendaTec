using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Portal.Helper;
using AgendaTec.Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AgendaTec.Portal.Controllers
{
    [Authorize]
    public class ImportUsersController : Controller
    {
        private readonly IUserFacade _userFacade;
        private readonly ICustomerFacade _customerFacade;

        public ImportUsersController(IUserFacade userFacade, ICustomerFacade customerFacade)
        {
            _userFacade = userFacade;
            _customerFacade = customerFacade;
        }       

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FineUploaderResult UploadFile(FineUpload uploadFile)
        {
            if (!(Path.GetExtension(uploadFile.Filename).Equals(".xls") || Path.GetExtension(uploadFile.Filename).Equals(".xlsx")))
                return new FineUploaderResult(false, error: "Tipo de arquivo inválido");

            if (uploadFile != null && uploadFile.InputStream.Length.Equals(0))
                return new FineUploaderResult(false, error: "Arquivo vazio ou corrompido.");

            var folder = new DirectoryInfo($"{Server.MapPath(@"\")}Uploads\\Users");
            var filePath = Path.Combine(folder.ToString(), uploadFile.Filename);

            try
            {
                uploadFile.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: $"Erro ao fazer upload do arquivo: {ex.Message} - {ex.InnerException}");
            }

            var fileNameSplit = uploadFile.Filename.Split('_');
            if (!fileNameSplit.Any())
                return new FineUploaderResult(false, error: $"Padrão de nomenclatura inválido: {uploadFile.Filename}.");

            try
            {              
                var idCustomer = int.Parse(fileNameSplit.First());
                var users = _userFacade.ReadUserFile(idCustomer, filePath, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return new FineUploaderResult(false, error: $"Ocorreu um erro ao processar o arquivo: {errorMessage}");

                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

                users.ForEach(newUser =>
                {
                    var checkResult = _userFacade.CheckDuplicatedUser(newUser, out errorMessage);
                    var checkCPFRequired = _customerFacade.CheckCPFRequired(idCustomer, newUser.CPF.CleanMask());

                    if (string.IsNullOrEmpty(checkResult) && checkCPFRequired)
                    {                    
                        var user = new ApplicationUser
                        {                            
                            IDRole = newUser.IdRole,
                            FirstName = newUser.FirstName,
                            LastName = newUser.LastName,
                            CPF = newUser.CPF.CleanMask(),
                            UserName = newUser.Email,
                            Email = newUser.Email,
                            PhoneNumber = newUser.Phone.CleanMask(),
                            IsEnabled = true,
                            DirectMail = true
                        };

                        var result = userManager.Create(user, "AgendaTec123");
                        if (result.Succeeded)
                        {
                            var e = new UserAssociatedCustomerDTO()
                            {
                                IDCustomer = idCustomer,
                                Email = newUser.Email
                            };

                            _userFacade.CheckUserAssociatedWithCustomer(e, out errorMessage);
                        }                        
                    }
                });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: $"Erro ao processar o arquivo: {ex.Message} - {ex.InnerException}");
            }

            return new FineUploaderResult(true);
        }
    }
}