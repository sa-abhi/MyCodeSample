using PMS_HT.Areas.BanquetLatest.Gateway;
using PMS_HT.Areas.CRM.Models;
using PMS_HT.Models;
using PMS_HT.Utility;
using PMS_HT.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PMS_HT.Gateway;


namespace PMS_HT.Areas.CRM.Gateway
{
    public class LeadsGateway : BaseGateway
    {
        public async Task<int> AddAsync(tbl_crm_leads leads)
        {
            if(leads == null)
            {
                return -1;
            }
            else
            {
                Db.tbl_crm_leads.Add(leads);
                int rowAffect = await Db.SaveChangesAsync().ConfigureAwait(false);
                return rowAffect;
            }
        }

        public async Task<int> EditAsync(tbl_crm_leads updatedLeads)
        {
            if(updatedLeads == null)
            {
                return -1;
            }
            else
            {
                Db.tbl_crm_leads.AddOrUpdate(updatedLeads);
                int rowAffect = await Db.SaveChangesAsync().ConfigureAwait(false);
                return rowAffect;
            }            
        }

        public async Task<int> DeleteByIdAsync(int? Id)
        {
            if(Id == null)
            {
                return -1;
            }
            else
            {
                tbl_crm_leads thisLead = await GetLeadByIdAsync(Id).ConfigureAwait(false);
                thisLead.ActionType = Constant.DELETE;
                Db.Entry(thisLead).State = EntityState.Modified;
                int rowAffect = await Db.SaveChangesAsync().ConfigureAwait(false);
                return rowAffect;
            }
        }

        public async Task<tbl_crm_leads> GetLeadByIdAsync(int? leadId)
        {
            if(leadId == null)
            {
                return null;
            }
            else
            {
                tbl_crm_leads findRow = await Db.tbl_crm_leads.FirstOrDefaultAsync(x => x.Lead_ID == 
leadId).ConfigureAwait(false);
                if(findRow == null)
                {
                    return null;
                }
                else
                {
                    return findRow;
                }
                
            }
        }        

        public async Task<List<tbl_country>> GetAllCountryAsync()
        {
            List<tbl_country> countryList = await Db.tbl_country.ToListAsync().ConfigureAwait(false);
            return countryList;
        }

        public async Task<List<tbl_crm_source>> GetAllSourceAsync()
        {
            List<tbl_crm_source> sourceList = await Db.tbl_crm_source.ToListAsync().ConfigureAwait(false);
            return sourceList;
        }

        public async Task<int> GetNumberOfContactedLeadsAsync(int? id, string permission)
        {
            int numberOfLeads;
            if (permission == Permissions.CRM.VIEW_ALL_LEADS)
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                 x.Status == Constant.LEAD_STATUS_CONTACTED && 
                                 x.ActionType != Constant.DELETE).ConfigureAwait(false);
            }
            else
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                 x.Generated_By == id && 
                                 x.Status == Constant.LEAD_STATUS_CONTACTED && 
                                 x.ActionType != Constant.DELETE).ConfigureAwait(false);

            }
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfProcessingLeadsAsync(int? id, string permission)
        {
            int numberOfLeads;
            if (permission == Permissions.CRM.VIEW_ALL_LEADS)
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                 x.Status == Constant.LEAD_STATUS_PROCESSING && 
                                 x.ActionType != Constant.DELETE).ConfigureAwait(false);
            }
            else
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                 x.Generated_By == id && 
                                 x.Status == Constant.LEAD_STATUS_PROCESSING && 
                                 x.ActionType != Constant.DELETE).ConfigureAwait(false);

            }
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfWonLeadsAsync(int? id, string permission)
        {
            int numberOfLeads;
            if (permission == Permissions.CRM.VIEW_ALL_LEADS)
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                       x.Status == Constant.LEAD_STATUS_WON && 
                                       x.ActionType != Constant.DELETE)
                                        .ConfigureAwait(false);
            }
            else
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                       x.Generated_By == id && 
                                       x.Status == Constant.LEAD_STATUS_WON && 
                                       x.ActionType != Constant.DELETE)
                                        .ConfigureAwait(false);
            }
            return numberOfLeads;
        }

        public async Task<int> GetNumberOfCustomerAsync(int? id, string permission)
        {
            int numberOfLeads;
            if (permission == Permissions.CRM.VIEW_ALL_LEADS)
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                       x.Status == Constant.LEAD_STATUS_CUSTOMER && 
                                       x.ActionType != Constant.DELETE)
                                        .ConfigureAwait(false);
            }
            else
            {                
                numberOfLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                       x.Generated_By == id && 
                                       x.Status == Constant.LEAD_STATUS_CUSTOMER && 
                                       x.ActionType != Constant.DELETE)
                                        .ConfigureAwait(false);

            }
            return numberOfLeads;
        }

        public async Task<int> GetMyTotalLeadsAsync(int? id)
        {            
            int totalLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                    x.Generated_By == id && 
                                    x.ActionType != Constant.DELETE)
                                     .ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetTotalLeadsAsync()
        {            
            int totalLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                    x.ActionType != Constant.DELETE)
                                     .ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetMyTotalLeadsTodayAsync(int? id)
        {            
            int totalLeads = await Db.tbl_crm_leads.CountAsync(x => 
                                    x.Generated_By == id && 
                          DbFunctions.TruncateTime(x.CreatedAt) == DateTime.Today && 
                                    x.ActionType != Constant.DELETE)
                                     .ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<int> GetTotalLeadsTodayAsync()
        {            
            int totalLeads = await Db.tbl_crm_leads.CountAsync(x => 
                          DbFunctions.TruncateTime(x.CreatedAt) == DateTime.Today && 
                                    x.ActionType != Constant.DELETE)
                                     .ConfigureAwait(false);
            return totalLeads;
        }

        public async Task<string> GetLeadEmailAsync(int? id)
        {
            tbl_crm_leads thisLead = await Db.tbl_crm_leads.FirstOrDefaultAsync(x => x.Lead_ID == id).ConfigureAwait(false);
            string email = thisLead.Email;
            return email;
        }

        public async Task<int> SaveProposalInfoAsync(tbl_crm_leads updateLead)
        {
            tbl_crm_leads thisLead = Db.tbl_crm_leads.FirstOrDefault(x => x.Lead_ID == updateLead.Lead_ID);
            thisLead.IsProposalSent = true;            
            thisLead.Proposal_File = updateLead.Proposal_File;
            Db.Entry(thisLead).State = EntityState.Modified;
            int rowAffect = await Db.SaveChangesAsync().ConfigureAwait(false);
            return rowAffect;
        }

        public async Task<List<tbl_crm_leads>> GetDateRangeAllLeadsAsync(DateTime? from, DateTime? to)
        {            
            List<tbl_crm_leads> leadsList = new List<tbl_crm_leads>();
            leadsList = await Db.tbl_crm_leads.Where(x => x.CreatedAt >= from && x.CreatedAt < to && x.ActionType != Constant.DELETE)
                                        .OrderBy(o => o.CreatedAt)
                                        .ToListAsync();
            return leadsList;
        }

        public async Task<List<tbl_crm_leads>> GetDateRangeOwnLeadsAsync(DateTime? from, DateTime? to, int? id)
        {            
            List<tbl_crm_leads> leadsList = new List<tbl_crm_leads>();
            leadsList = await Db.tbl_crm_leads.Where(x => x.CreatedAt >= from && x.CreatedAt < to && x.Generated_By == id && x.ActionType != Constant.DELETE)
                                        .OrderBy(o => o.CreatedAt)
                                        .ToListAsync();
            return leadsList;
        }


        public async Task<List<tbl_crm_leads>> GetDateRangeIndividualLeadsAsync(DateTime? from, DateTime? to, string type, int? id)
        {
            List<tbl_crm_leads> leadList = new List<tbl_crm_leads>();
            if (type == Constant.LEAD_STATUS_ALL)
            {
                leadList = await Db.tbl_crm_leads.Where(x =>
                                                       x.Generated_By == id &&                                                       
                                                       x.ActionType != Constant.DELETE)
                                                        .OrderBy(o => o.CreatedAt)
                                                        .ToListAsync();
            }
            else
            {
                leadList = await Db.tbl_crm_leads.Where(x =>
                                                      x.Generated_By == id &&
                                                      x.Status == type &&
                                                      x.ActionType != Constant.DELETE)
                                                       .OrderBy(o => o.CreatedAt)
                                                       .ToListAsync();
            }           
            return leadList;
        }

        public bool isEmailExist(string email)
        {            
            tbl_crm_leads hasRow = Db.tbl_crm_leads.FirstOrDefault(x => x.Email == email);
            if(hasRow == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}