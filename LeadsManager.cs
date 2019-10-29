using PMS_HT.Areas.CRM.Gateway;
using PMS_HT.Areas.CRM.Models;
using PMS_HT.Models;
using PMS_HT.Utility;
using PMS_HT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PMS_HT.Areas.CRM.Manager
{
    public class LeadsManager
    {
        private LeadsGateway leadsGateway;

        public LeadsManager()
        {
            leadsGateway = new LeadsGateway();
        }

        public async Task<ResponseMessage> AddAsync(tbl_crm_leads leads)
        {
            bool isEmailExist = leadsGateway.isEmailExist(leads.Email);
            int rowAffect = await leadsGateway.AddAsync(leads).ConfigureAwait(false);
            if (isEmailExist || rowAffect <= 0)
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_FAILED,
                    Message = GeneralMessages.SAVE_FAILED
                };
            }
            else
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_SUCCESS,
                    Message = GeneralMessages.SAVE_SUCCESSFUL
                };
            }
        }

        public async Task<ResponseMessage> EditAsync(tbl_crm_leads updateLeads)
        {
            int rowAffect = await leadsGateway.EditAsync(updateLeads).ConfigureAwait(false);
            if(rowAffect <= 0)
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_FAILED,
                    Message = GeneralMessages.SAVE_FAILED
                };
            }
            else
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_SUCCESS,
                    Message = GeneralMessages.SAVE_SUCCESSFUL
                };
            }
        }

        public async Task<ResponseMessage> DeleteByIdAsync(int? id)
        {
            if (id == null) 
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_FAILED,
                    Message = GeneralMessages.SAVE_FAILED
                };
            }
            else
            {
                int rowAffect = await leadsGateway.DeleteByIdAsync(id).ConfigureAwait(false);
                if(rowAffect <= 0)
                {
                    return new ResponseMessage
                    {
                        Type = Constant.RESPONSE_MESSAGE_TYPE_FAILED,
                        Message = GeneralMessages.SAVE_FAILED
                    };
                }
                else
                {
                    return new ResponseMessage
                    {
                        Type = Constant.RESPONSE_MESSAGE_TYPE_SUCCESS,
                        Message = GeneralMessages.SAVE_SUCCESSFUL
                    };
                }
            }
        }

        public async Task<tbl_crm_leads> GetLeadByIdAsync(int? leadId)
        {
            if (leadId == null)
            {
                return null;
            }
            else
            {
                tbl_crm_leads findLead = await leadsGateway.GetLeadByIdAsync(leadId).ConfigureAwait(false);
                return findLead;
            }
        }

        public async Task<List<tbl_country>> GetAllCountryAsync()
        {
            List<tbl_country> countryList = await leadsGateway.GetAllCountryAsync().ConfigureAwait(false);
            return countryList;
        }

        public async Task<List<tbl_crm_source>> GetAllSourceAsync()
        {
            List<tbl_crm_source> sourceList = await leadsGateway.GetAllSourceAsync().ConfigureAwait(false);
            return sourceList;
        }

        public async Task<int> GetNumberOfContactedLeadsAsync(int? id, string permission)
        {
            int numberOfLeads = await leadsGateway.GetNumberOfContactedLeadsAsync(id, permission).ConfigureAwait(false);
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfProcessingLeadsAsync(int? id, string permission)
        {
            int numberOfLeads = await leadsGateway.GetNumberOfProcessingLeadsAsync(id, permission).ConfigureAwait(false);
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfWonLeadsAsync(int? id, string permission)
        {
            int numberOfLeads = await leadsGateway.GetNumberOfWonLeadsAsync(id, permission).ConfigureAwait(false);
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfCustomerAsync(int? id, string permission)
        {
            int numberOfLeads = await leadsGateway.GetNumberOfCustomerAsync(id, permission).ConfigureAwait(false);
            return numberOfLeads;
        }

        public async Task<int> GetMyTotalLeadsAsync(int? id)
        {
            int totalLeads = await leadsGateway.GetMyTotalLeadsAsync(id).ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetTotalLeadsAsync()
        {
            int totalLeads = await leadsGateway.GetTotalLeadsAsync().ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetMyTotalLeadsTodayAsync(int? id)
        {
            int totalLeads = await leadsGateway.GetMyTotalLeadsTodayAsync(id).ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetTotalLeadsTodayAsync()
        {
            int totalLeads = await leadsGateway.GetTotalLeadsTodayAsync().ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<string> GetLeadEmailAsync(int? id)
        {
            string email = await leadsGateway.GetLeadEmailAsync(id).ConfigureAwait(false);
            return email;
        }

        public async Task<ResponseMessage> SaveProposalInfoAsync(tbl_crm_leads updateLead)
        {
            int rowAffect = await leadsGateway.SaveProposalInfoAsync(updateLead).ConfigureAwait(false);
            if(rowAffect <= 0)
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_FAILED
                };                
            }
            else
            {
                return new ResponseMessage
                {
                    Type = Constant.RESPONSE_MESSAGE_TYPE_SUCCESS
                };
            }            
        }

        public async Task<List<tbl_crm_leads>> GetDateRangeAllLeadsAsync(DateTime? from, DateTime? to)
        {
            List<tbl_crm_leads> leadsList = new List<tbl_crm_leads>();
            leadsList = await leadsGateway.GetDateRangeAllLeadsAsync(from, to).ConfigureAwait(false);
            return leadsList;
        }

        public async Task<List<tbl_crm_leads>> GetDateRangeOwnLeadsAsync(DateTime? from, DateTime? to, int? id)
        {            
            List<tbl_crm_leads> leadsList = new List<tbl_crm_leads>();
            leadsList = await leadsGateway.GetDateRangeOwnLeadsAsync(from, to, id).ConfigureAwait(false);
            return leadsList;
        }

        public async Task<List<tbl_crm_leads>> GetDateRangeIndividualLeadsAsync(DateTime? from, DateTime? to, string type, int? id)
        {
            List<tbl_crm_leads> leadList = new List<tbl_crm_leads>();
            leadList = await leadsGateway.GetDateRangeIndividualLeadsAsync(from, to, type, id).ConfigureAwait(false);
            return leadList;
        }

    }
}