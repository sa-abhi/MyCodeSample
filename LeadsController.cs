using PMS_HT.Areas.CRM.Manager;
using PMS_HT.Areas.CRM.Models;
using PMS_HT.Controllers;
using PMS_HT.Utility;
using PMS_HT.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using System.Net;
using Microsoft.Reporting.WebForms;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Net.Mime;
using PMS_HT.Models;

namespace PMS_HT.Areas.CRM.Controllers
{
    public class LeadsController : CustomControllerBase
    {
        private LeadsManager leadsManager;        
        private CRM_Task_Manager taskManager;

        public LeadsController()
        {
            leadsManager = new LeadsManager();
            taskManager = new CRM_Task_Manager();
        }

        public async Task<ActionResult> CrmDashboard()
        {
            tbl_emp_info loggedInUserInfo = Utilities.GetLoggedInUserInfo(User.Identity.Name);
            int? loggedInUser = loggedInUserInfo.Emp_ID;
            int myTotalLeads = 0;
            int myTotalLeadsToday = 0;
            int TotalLeads = 0;
            int TotalLeadsToday = 0;
            int numberOfHighPrioritytask = 0;
            int totalTask = 0;
            List<tbl_crm_task> todaysCallList = new List<tbl_crm_task>();
            List<tbl_crm_task> todaysAppointmentList = new List<tbl_crm_task>();
            List<tbl_crm_task> todaysOtherTaskList = new List<tbl_crm_task>();

            if (loggedInUser == null)
            {
                return new HttpUnauthorizedResult();
            }
            else
            {
                if (User.IsInRole(Permissions.CRM.VIEW_OWN_CRM_DASHBOARD))
                {
                    myTotalLeads = await leadsManager.GetMyTotalLeadsAsync(loggedInUser).ConfigureAwait(false);
                    myTotalLeadsToday = await leadsManager.GetMyTotalLeadsTodayAsync(loggedInUser).ConfigureAwait(false);
                }
                else if (User.IsInRole(Permissions.CRM.VIEW_CRM_DASHBOARD))
                {
                    TotalLeads = await leadsManager.GetTotalLeadsAsync().ConfigureAwait(false);
                    TotalLeadsToday = await leadsManager.GetTotalLeadsTodayAsync().ConfigureAwait(false);
                }

                if (User.IsInRole(Permissions.CRM.VIEW_OWN_TASKS))
                {
                    numberOfHighPrioritytask = await taskManager.GetNumberOfInCompleteHighPriorityTaskAsync(loggedInUser).ConfigureAwait(false);
                    totalTask = await taskManager.GetTotalIncompleteTaskAsync(loggedInUser).ConfigureAwait(false);
                    todaysCallList = await taskManager.GetTodaysCallListAsync(loggedInUser).ConfigureAwait(false);
                    todaysAppointmentList = await taskManager.GetTodaysAppointmentListAsync(loggedInUser).ConfigureAwait(false);
                    todaysOtherTaskList = await taskManager.GetTodaysOtherTaskListAsync(loggedInUser).ConfigureAwait(false);
                }               

                ViewBag.MyTotalLeads = myTotalLeads;
                ViewBag.MyTotalLeadsToday = myTotalLeadsToday;
                ViewBag.TotalLeads = TotalLeads;
                ViewBag.TotalLeadsToday = TotalLeadsToday;
                ViewBag.HighPriorityTask = numberOfHighPrioritytask;
                ViewBag.TotalTask = totalTask;
                ViewBag.TodaysCallList = todaysCallList;
                ViewBag.TodaysAppointmentList = todaysAppointmentList;
                ViewBag.TodaysOtherTaskList = todaysOtherTaskList;
                return View();
            }            
        }

        public async Task<ActionResult> Index()
        {
            tbl_emp_info loggedInUserInfo = Utilities.GetLoggedInUserInfo(User.Identity.Name);
            int? loggedInUser = loggedInUserInfo.Emp_ID;
            string permission = "";

            if (loggedInUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            List<tbl_crm_leads> allLeadList = new List<tbl_crm_leads>();

            if ((!User.IsInRole(Permissions.CRM.VIEW_ALL_LEADS)) && (User.IsInRole(Permissions.CRM.VIEW_OWN_LEADS)))
            {
                allLeadList = await db.tbl_crm_leads.Where(i => i.Generated_By == loggedInUser && i.ActionType != Constant.DELETE).ToListAsync().ConfigureAwait(false);
                permission = Permissions.CRM.VIEW_OWN_LEADS;
            }
            else if(User.IsInRole(Permissions.CRM.VIEW_ALL_LEADS))
            {                
                allLeadList = await db.tbl_crm_leads.Where(i => i.ActionType != Constant.DELETE).ToListAsync().ConfigureAwait(false);
                permission = Permissions.CRM.VIEW_ALL_LEADS;
            }            

            ViewBag.NumberOfContactedLeads = await leadsManager.GetNumberOfContactedLeadsAsync(loggedInUser, permission).ConfigureAwait(false);
            ViewBag.NumberOfProcessingLeads = await leadsManager.GetNumberOfProcessingLeadsAsync(loggedInUser, permission).ConfigureAwait(false);
            ViewBag.NumberOfWonLeads = await leadsManager.GetNumberOfWonLeadsAsync(loggedInUser, permission).ConfigureAwait(false);
            ViewBag.NumberOfCustomer = await leadsManager.GetNumberOfCustomerAsync(loggedInUser, permission).ConfigureAwait(false);

            return View(allLeadList);
        }

        [Authorize(Roles = Permissions.CRM.ADD_LEADS)]
        public ActionResult Add()
        {
            ViewBag.Country_ID = new SelectList(db.tbl_country.ToList(), "Country_ID", "Country_Name");
            ViewBag.Source_ID = new SelectList(db.tbl_crm_source.ToList(), "Source_ID", "Source_Name");            
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Add(LeadsViewModel Lead)
        {            
            if (ModelState.IsValid)
            {
                tbl_crm_leads newLead = Mapper.Map<LeadsViewModel, tbl_crm_leads>(Lead);
                tbl_emp_info loggedInUserInfo = Utilities.GetLoggedInUserInfo(User.Identity.Name);
                int? loggedInUser = loggedInUserInfo.Emp_ID;
                newLead.Generated_By = loggedInUser;
                newLead.CreatedAt = DateTime.Now;
                ResponseMessage responseMessage = await leadsManager.AddAsync(newLead).ConfigureAwait(false);
               if(responseMessage.Type == Constant.RESPONSE_MESSAGE_TYPE_SUCCESS)
                {
                    return Json(GeneralMessages.SAVE_SUCCESSFUL, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(GeneralMessages.SAVE_FAILED, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(GeneralMessages.SAVE_FAILED, JsonRequestBehavior.AllowGet);                
            }
        }

        [HttpGet]
        [Authorize(Roles = Permissions.CRM.EDIT_LEADS)]
        public async Task<ActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return Json(false);
            }
            else
            {
                tbl_crm_leads findLead =  await leadsManager.GetLeadByIdAsync(id).ConfigureAwait(false);
                LeadsEditViewModel leadEditViewModel = Mapper.Map<tbl_crm_leads, LeadsEditViewModel>(findLead);                
                leadEditViewModel.CountryList = await leadsManager.GetAllCountryAsync().ConfigureAwait(false);
                leadEditViewModel.SourceList = await leadsManager.GetAllSourceAsync().ConfigureAwait(false);
                return PartialView(leadEditViewModel);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(LeadsEditViewModel updateLeads)
        {
            if (ModelState.IsValid)
            {
                tbl_crm_leads updatedLead = Mapper.Map<LeadsEditViewModel, tbl_crm_leads>(updateLeads);
                tbl_emp_info loggedInUserInfo = Utilities.GetLoggedInUserInfo(User.Identity.Name);
                int? loggedInUser = loggedInUserInfo.Emp_ID;
                updatedLead.Generated_By = loggedInUser;
                updatedLead.UpdatedAt = DateTime.Now;
                ResponseMessage responseMessage = await leadsManager.EditAsync(updatedLead).ConfigureAwait(false);
                if(responseMessage.Type == Constant.RESPONSE_MESSAGE_TYPE_SUCCESS)
                {
                    return Json(GeneralMessages.SAVE_SUCCESSFUL, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(GeneralMessages.SAVE_FAILED, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(GeneralMessages.SAVE_FAILED, JsonRequestBehavior.AllowGet);
            }
           
            
        }

        [Authorize(Roles = Permissions.CRM.DELETE_LEADS)]
        public async Task<ActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResponseMessage responseMessage = await leadsManager.DeleteByIdAsync(id).ConfigureAwait(false);
            if(responseMessage.Type == Constant.RESPONSE_MESSAGE_TYPE_SUCCESS)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_crm_leads viewLead = await leadsManager.GetLeadByIdAsync(id).ConfigureAwait(false);
            if (viewLead == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return PartialView(viewLead);
            }
            
        }

        [HttpGet]
        public async Task<ActionResult> SendProposal(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                string email = await leadsManager.GetLeadEmailAsync(id).ConfigureAwait(false);
                SendProposalViewModel sendViewModel = new SendProposalViewModel();
                sendViewModel.Lead_ID = id;
                sendViewModel.To = email;                
                return PartialView(sendViewModel);
            }            
        }

        [HttpPost]           
        public async Task<JsonResult> SaveProposalInfo(SendProposalViewModel viewModel)
        {            
            var postedFileWithoutExtension = Path.GetFileNameWithoutExtension(viewModel.Attachment.FileName);
            viewModel.Proposal_File = Utilities.SaveImage(Server, viewModel.Attachment, postedFileWithoutExtension, Constant.FILE_PATH_FOR_LEADS_PROPOSAL);
            tbl_crm_leads updateLead = Mapper.Map<SendProposalViewModel, tbl_crm_leads>(viewModel);

            ResponseMessage responseMessage = await leadsManager.SaveProposalInfoAsync(updateLead).ConfigureAwait(false);
            if (responseMessage.Type == Constant.RESPONSE_MESSAGE_TYPE_SUCCESS)
            {
                var root = Server.MapPath(Constant.FILE_PATH_FOR_LEADS_PROPOSAL);
                var fileName = viewModel.Proposal_File;
                var path = Path.Combine(root, fileName);
                path = Path.GetFullPath(path);

                var message = new MailMessage();
                message.To.Add(new MailAddress(viewModel.To));  // replace with valid value 
                message.From = new MailAddress(ConfigurationManager.AppSettings["CRM_EMAIL_ADDRESS"]);  // replace with valid value
                message.Subject = viewModel.Subject;
                message.Body = viewModel.Message;
                Attachment attachment = new Attachment(path, MediaTypeNames.Application.Octet);
                System.Net.Mime.ContentDisposition disposition = attachment.ContentDisposition;
                message.Attachments.Add(attachment);

                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["CRM_EMAIL_ADDRESS"],  // replace with valid value
                        Password = ConfigurationManager.AppSettings["CRM_EMAIL_PASSWORD"]  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }

                return new JsonResult { Data = new { status = true } };
            }
            else
            {
                return new JsonResult { Data = new { status = false } };
            }
        }

        //REPORT SECTION START        
        public JsonResult ExportLeadInfo(int? id)
        {
            if (id == null)
            {
                return Json(false);
            }
            //Step 1 - Get data according to your requirement for report
            var leadInfo = db.tbl_crm_leads.Where(x => x.Lead_ID == id).Select(a => new
            {
                FirstName = a.First_Name,
                MiddleNam = a.Middle_Name ?? "N/A",
                LastName = a.Last_Name,
                Country = a.tbl_country.Country_Name,
                Address = a.Address,
                Email = a.Email,
                Comapany = a.Company ?? "N/A",
                Designaion = a.Designation ?? "N/A",
                Website = a.Website ?? "N/A",
                Source = a.TblCrmSource.Source_Name,
                ContactNo = a.Contact_No ?? "N/A",
                ReferredBy = a.referred_by_emp_info.Emp_Name ?? "",
                ExistingOffer = a.Existing_Offer ?? "N/A",
                ExpectedOffer = a.Expected_Offer ?? "N/A",
                Feedback = a.Feedback ?? "N/A",
                Note = a.Note ?? "N/A",
                Status = a.Status
            });

            string reportName = "LeadInfoReport";
            string dataSetName = "LeadInfoDataSet";
            String reportData = Utilities.CreateReport(this, reportName, dataSetName, leadInfo);
            return Json(reportData, JsonRequestBehavior.AllowGet);            
        }
        //REPORT SECTION END

        [HttpGet]
        public ActionResult SearchLeads()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ViewDateRangeLeads(DateTime? from, DateTime? to)
        {
            if (from == null || to == null || from > to)
            {
                return Json("Invalid", JsonRequestBehavior.AllowGet);
            }
            to = to.Value.AddDays(1);

            tbl_emp_info loggedInUserInfo = Utilities.GetLoggedInUserInfo(User.Identity.Name);
            int? loggedInUser = loggedInUserInfo.Emp_ID;

            if (loggedInUser == null)
            {
                return new HttpUnauthorizedResult();
            }
            List<tbl_crm_leads> leadsList = new List<tbl_crm_leads>();
            if (User.IsInRole(Permissions.CRM.VIEW_ALL_LEADS))
            {
                leadsList = await leadsManager.GetDateRangeAllLeadsAsync(from, to).ConfigureAwait(false);
            }
            else if (User.IsInRole(Permissions.CRM.VIEW_OWN_LEADS))
            {
                leadsList = await leadsManager.GetDateRangeOwnLeadsAsync(from, to, loggedInUser).ConfigureAwait(false);
            }
            return PartialView(leadsList);
        }

    }


}