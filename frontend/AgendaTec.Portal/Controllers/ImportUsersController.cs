using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
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

        public ImportUsersController(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }
       

        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult UploadImage(HttpPostedFileBase uploadFile)
        //{
        //    var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        //    if (uploadFile != null)
        //    {
        //        string ImageName = System.IO.Path.GetFileName(uploadFile.FileName);
        //        string Path = Server.MapPath("~/Content/Images/" + ImageName);
        //        // save image in folder
        //        uploadFile.SaveAs(Path);
              


        //    }
        //    return View();
        //}

        public ActionResult ImportUsers(HttpPostedFileBase uploadFile)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //bool isSavedSuccessfully = true;
            //string fName = "";
            try
            {
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                    string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);

                    var path = string.Format("{0}\\{1}", pathString, uploadFile.FileName);

                    uploadFile.SaveAs(path);

                    var fileNameSplit = uploadFile.FileName.Split('_');
                    if (fileNameSplit.Any())
                    {
                        var idCustomer = int.Parse(fileNameSplit.First());
                        var users = _userFacade.ReadUserFile(idCustomer, path, out string errorMessage);

                        if (users.Any() && string.IsNullOrEmpty(errorMessage))
                        {
                            users.ForEach(newUser =>
                            {
                                var checkResult = _userFacade.CheckDuplicatedUser(newUser, out errorMessage);
                                // if (!string.IsNullOrEmpty(errorMessage))
                                //   return Json(new { Success = false, errorMessage = "Houve um erro ao salvar o usuário." }, JsonRequestBehavior.AllowGet);

                                if (string.IsNullOrEmpty(checkResult))
                                {
                                        // return Json(new { Success = false, errorMessage = checkResult }, JsonRequestBehavior.AllowGet);

                                        //if (!_customerFacade.CheckCPFRequired(int.Parse(userDTO.IDCustomer), userDTO.CPF))
                                        //  return Json(new { Success = false, errorMessage = "CPF inválido." }, JsonRequestBehavior.AllowGet);

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

                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {


                                        }
                                    }
                                    else
                                    {


                                    }
                                }
                            });
                        }
                        else
                        {
                          //  isSavedSuccessfully = false;
                        }
                    }
                    else
                    {
                        //    isSavedSuccessfully = false;
                    }                   
                }
            }
            catch (Exception ex)
            {
              //  isSavedSuccessfully = false;
            }

            return View("Index");
        }
    }
}