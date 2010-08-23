using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.controls
{
    // YAF.Pages
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Extensions;
    using YAF.Classes.UI;
    using YAF.Classes.Utils;
    using YAF.Controls;
    using YAF.Editors;
    using YAF.Utilities;

    #endregion

    public partial class PollList : BaseUserControl
    {
        /// <summary>
        ///   The _data bound.
        /// </summary>
        private bool _dataBound;
  

        /// <summary>
        ///   The _dt poll.
        /// </summary>
        private DataTable _dtPoll;

        /// <summary>
        ///   The _dt PollGroup.
        /// </summary>
        private DataTable _dtPollGroup;

        /// <summary>
        ///   The _dt Votes.
        /// </summary>
        private DataTable _dtVotes;

        /// <summary>
        ///   The topic User.
        /// </summary>
        private int? topicUser;

        /// <summary>
        ///   The _canChange.
        /// </summary>
        private bool _canChange;

        /// <summary>
        ///   The isBound.
        /// </summary>
        private bool isBound;

        /// <summary>
        /// Returns PollGroupID
        /// </summary>
        public int? PollGroupId { get; set; }

        /// <summary>
        /// Returns TopicId
        /// </summary>
        public int TopicId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditMessageId
        /// </summary>
        public int EditMessageId
        {
            get;
            set;
        }
        
        /// <summary>
        /// Returns CategoryId
        /// </summary>
        public int CategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditCategoryId
        /// </summary>
        public int EditCategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns ForumId
        /// </summary>
        public int ForumId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditForumId
        /// </summary>
        public int EditForumId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns BoardId
        /// </summary>
        public int BoardId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditBoardId
        /// </summary>
        public int EditBoardId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns PollNumber
        /// </summary>
        public int PollNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Returns IsLocked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Returns If we are editing a post
        /// </summary>
        public bool PostEdit { get; set; }

        /// <summary>
        /// Returns ShowButtons
        /// </summary>
        public bool ShowButtons { get; set; }
       

        /// <summary>
        ///   Gets VotingCookieName.
        /// </summary>
        protected string VotingCookieName(int pollId)
        {
          
                return String.Format("poll#{0}", pollId);
           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           // if (!IsPostBack)
          //  {
                this.NewPollRow.Visible = HasOwnerExistingGroupAccess();

                if (this.TopicId > 0)
                {
                    topicUser = Convert.ToInt32(DB.topic_info(this.TopicId)["UserID"]);
                }
                if (PollGroupId > 0)
                {
                    BindData();
                }
                else
                {
                    if (this.NewPollRow.Visible)
                    {
                        BindCreateNewPollRow();
                    }
                }
        //    }

        }

        private bool HasOwnerExistingGroupAccess()
        { 

            if ((PageContext.BoardSettings.AllowedPollChoiceNumber > 0) && ShowButtons && (this.PollGroupId >= 0))
            {
                // it topicid > 0 it can be every member
                if (this.TopicId > 0)
                {
                     return (topicUser == this.PageContext.PageUserID) || this.PageContext.IsAdmin || this.PageContext.IsForumModerator;
                   
                }

                // only admins can edit this
                if (this.CategoryId > 0 || this.BoardId > 0)
                {
                    return this.PageContext.IsAdmin;
                }
                // in other places only admins and forum moderators can have access
                else
                {
                   return this.PageContext.IsAdmin || this.PageContext.IsForumModerator;
                }
            }
            return false;
           
        }

        public string GetThemeContents(string page, string tag)
        {
            return this.PageContext.Theme.GetItem(page, tag);
        }


        private  void BindCreateNewPollRow()
        {
           
                var cpr = this.CreatePoll1;
                // this.ChangePollShowStatus(true);
                cpr.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.polledit,
                    "{0}",
                    ParamsToSend());
                cpr.DataBind();
            cpr.Visible = true;
            this.NewPollRow.Visible = true;
        }


        private string ParamsToSend()
        {
           StringBuilder sb = new StringBuilder();

            if (this.TopicId > 0)
            {
               sb.AppendFormat("t={0}", this.TopicId);
            }

            if (this.EditMessageId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("m={0}", this.EditMessageId);

            }

            if (this.ForumId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("f={0}", this.ForumId);
            }

            if (this.EditForumId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("ef={0}", this.EditForumId);
            }

            if (this.CategoryId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("c={0}", this.CategoryId);
            }

            if (this.EditCategoryId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("ec={0}", this.EditCategoryId);
            }

            if (this.BoardId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("b={0}", this.BoardId);
            }

            if (this.EditBoardId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("eb={0}", this.EditBoardId);
            }

            return sb.ToString();
          
        }


        protected bool IsNotVoted(object pollId)
        {

            // check for voting cookie
            if (this.Request.Cookies[VotingCookieName(Convert.ToInt32(pollId))] != null)
            {
                return false;
            }

            // voting is not tied to IP and they are a guest...
            if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.PollVoteTiedToIP)
            {
                return true;
            }

            // Check if a user already voted
            foreach (DataRow  dr in _dtVotes.Rows)
            {
                if (Convert.ToInt32(dr["PollID"]) == Convert.ToInt32(pollId))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Property to verify if the current user can vote in this poll.
        /// </summary>
        protected bool CanVote(object pollId)
        {
       
            // rule out users without voting rights
            if (!this.PageContext.ForumVoteAccess)
           {
                    return false;
           }
           if (this.IsPollClosed(pollId))
           {
               return false;
           }

            return IsNotVoted(pollId);
            
        }

        /// <summary>
        /// The get poll is closed.
        /// </summary>
        /// <returns>
        /// The get poll is closed.
        /// </returns>
        protected string GetPollIsClosed(object pollId)
        {
            string strPollClosed = string.Empty;
            if (this.IsPollClosed(pollId))
            {
                strPollClosed = this.PageContext.Localization.GetText("POLL_CLOSED");
            }

            return strPollClosed;
        }

        /// <summary>
        /// The get poll question.
        /// </summary>
        /// <returns>
        /// The get poll question.
        /// </returns>
        protected string GetPollQuestion(object pollId)
        {
            foreach (DataRow dr in this._dtPollGroup.Rows)
            {
                if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    return this.HtmlEncode(YafServices.BadWordReplace.Replace(dr["Question"].ToString()));
                }
                
            }
            return string.Empty;
           
        }

        /// <summary>
        /// The get total.
        /// </summary>
        /// <returns>
        /// The get total.
        /// </returns>
        protected string GetTotal(object  pollId)
        {
            foreach (DataRow dr in this._dtPollGroup.Rows)
            {
                if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    return this.HtmlEncode(dr["Total"].ToString());
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// The is poll closed.
        /// </summary>
        /// <returns>
        /// The is poll closed.
        /// </returns>
        protected bool IsPollClosed(object pollId)
        {

            foreach (DataRow dr in this._dtPollGroup.Rows)
            {
                if ((dr["Closes"] != DBNull.Value) && (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"])))
                {
                    DateTime tCloses = Convert.ToDateTime(dr["Closes"]);
                    if (tCloses < DateTime.UtcNow)
                    {
                        return true;
                    }
                    
                }

            }
  
            return false;
        }

        /// <summary>
        /// The days to run.
        /// </summary>
        /// <returns>
        /// The days to run.
        /// </returns>
        protected int? DaysToRun(object pollId, out bool soon)
        {
            soon = false;
            foreach (DataRow dr in this._dtPollGroup.Rows)
            {
                if (dr["Closes"] != DBNull.Value && Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    DateTime tCloses = Convert.ToDateTime(dr["Closes"]).Date; 
                    if (tCloses > DateTime.UtcNow.Date)
                    {
                        int days = (tCloses - DateTime.UtcNow).Days;
                        if (days == 0)
                        {
                            return 1;
                            soon = true;
                        }
                        else
                        {
                            return days;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            return null;
        }


     
        /// <summary>
        /// The vote width.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The vote width.
        /// </returns>
        protected int VoteWidth(object o)
        {
            var row = (System.Data.DataRowView)o;
            return (int)row.Row["Stats"] * 80 / 100;
        }

        /// <summary>
        /// The PollGroup item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PollGroup_OnItemDataBound(object source, RepeaterItemEventArgs e)
        {
           
            RepeaterItem item = e.Item;
            
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {

               item.FindControlRecursiveAs<HtmlTableRow>("PollCommandRow").Visible = HasOwnerExistingGroupAccess() && ShowButtons;

                Repeater polloll = item.FindControlRecursiveAs<Repeater>("Poll");

                string pollId = item.FindControlRecursiveAs<HiddenField>("PollID").Value;
                polloll.Visible = !CanVote(pollId) && !this.PageContext.BoardSettings.AllowGuestsViewPollOptions && this.PageContext.IsGuest
                                    ? false
                                    : true;
                DataTable _choiceRow = _dtPoll.Copy();
                foreach (DataRow drr in _choiceRow.Rows)
                {
                    if (Convert.ToInt32(drr["PollID"]) != Convert.ToInt32(pollId))
                    {
                        drr.Delete();
                    }
                }
                
                polloll.DataSource = _choiceRow;
      
                polloll.DataBind();

                ThemeButton removePollAll = item.FindControlRecursiveAs<ThemeButton>("RemovePollAll");
                removePollAll.Attributes["onclick"] = String.Format(
               "return confirm('{0}');", this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
                removePollAll.Visible = CanRemovePollCompletely(pollId);
               
                ThemeButton removePoll = item.FindControlRecursiveAs<ThemeButton>("RemovePoll");
                removePoll.Attributes["onclick"] = String.Format(
                         "return confirm('{0}');", this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE"));
                removePoll.Visible = CanRemovePoll(pollId);

                // Poll warnings section
                bool isNotVoted = IsNotVoted(pollId);
                bool soon;

                int? daystorun = DaysToRun(pollId, out soon);

                Label pollVotesLabel = item.FindControlRecursiveAs<Label>("PollVotesLabel");
                bool cvote = CanVote(pollId);
                if (cvote)
                {
                    if (this.isBound && this.PollNumber > 1 && this.PollNumber >= this._dtVotes.Rows.Count)
                    {
                        pollVotesLabel.Text = this.PageContext.Localization.GetText("POLLEDIT", "POLLGROUP_BOUNDWARN");
                    }
                    if (!this.PageContext.BoardSettings.AllowUsersViewPollVotesBefore)
                    {

                        if (!PageContext.IsGuest)
                        {
                            pollVotesLabel.Text += this.PageContext.Localization.GetText("POLLEDIT", "POLLRESULTSHIDDEN");
                        }
                        else
                        {
                            pollVotesLabel.Text += this.PageContext.Localization.GetText("POLLEDIT",
                                                                                         "POLLRESULTSHIDDEN_GUEST");
                        }
                    }
                }

                if (PageContext.IsGuest)
                {
                    Label guestOptionsHidden = item.FindControlRecursiveAs<Label>("GuestOptionsHidden");
                    if (!cvote &&  (!this.PageContext.BoardSettings.AllowGuestsViewPollOptions))
                    {
                        guestOptionsHidden.Text = this.PageContext.Localization.GetText("POLLEDIT",
                                                                                        "POLLOPTIONSHIDDEN_GUEST");
                        guestOptionsHidden.Visible = true;
                    }
                    if (!PageContext.ForumPollAccess)
                    {
                        guestOptionsHidden.Text += this.PageContext.Localization.GetText("POLLEDIT",
                                                                                    "POLL_NOPERM_GUEST");
                        guestOptionsHidden.Visible = true;   
                    }
                }



                pollVotesLabel.Visible = this.isBound || (this.PageContext.BoardSettings.AllowUsersViewPollVotesBefore
                                             ? false
                                             : (isNotVoted || (daystorun == null)));

                if (!isNotVoted && PageContext.ForumPollAccess)
                 {
                     Label alreadyVotedLabel = item.FindControlRecursiveAs<Label>("AlreadyVotedLabel");
                     alreadyVotedLabel.Text = this.PageContext.Localization.GetText("POLLEDIT", "POLL_VOTED");
                     alreadyVotedLabel.Visible = true;
                 } 

                if (daystorun > 0)
                {
                    Label pollWillExpire = item.FindControlRecursiveAs<Label>("PollWillExpire");
                    if (!soon)
                    {
                        pollWillExpire.Text = this.PageContext.Localization.GetTextFormatted("POLL_WILLEXPIRE",
                                                                                             daystorun);
                    }
                    else
                    {
                        pollWillExpire.Text = this.PageContext.Localization.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS");
                    }

                    pollWillExpire.Visible = true;
                }
                else if (daystorun == 0)
                {

                    Label pollExpired = item.FindControlRecursiveAs<Label>("PollExpired");
                    pollExpired.Text = this.PageContext.Localization.GetText("POLLEDIT", "POLL_EXPIRED");
                    pollExpired.Visible = true;

                }
                DisplayButtons();
           }

            if (item.ItemType == ListItemType.Footer)
            {
                var pgcr = item.FindControlRecursiveAs<HtmlTableRow>("PollGroupCommandRow");
                pgcr.Visible = HasOwnerExistingGroupAccess() && ShowButtons;
               
                if (pgcr.Visible)
                {
                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroup").Attributes["onclick"] = String.Format(
                        "return confirm('{0}');",
                        this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLGROUP_DELETE"));

                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroupAll").Attributes["onclick"] = String.Format(
                        "return confirm('{0}');",
                        this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_ALL"));

                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroupEverywhere").Attributes["onclick"] = String.
                        Format(
                            "return confirm('{0}');",
                            this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_EVR"));
                }

            }




            //  }

        }


        private void DisplayButtons()
        {

           // this.PollGroup.FindControlRecursiveAs<HtmlTableRow>("PollCommandRow").Visible = this.ShowButtons;

        }

        protected void PollGroup_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
         
            if (e.CommandName == "new" && this.PageContext.ForumVoteAccess)
            {
                YafBuildLink.Redirect(
                   ForumPages.polledit,
                   "{0}",
                  ParamsToSend()
                   );
            }
            if (e.CommandName == "edit" && this.PageContext.ForumVoteAccess)
            {
                YafBuildLink.Redirect(
                    ForumPages.polledit,
                    "{0}&p={1}",
                     ParamsToSend(),
                    e.CommandArgument.ToString());
                
            }
            if (e.CommandName == "remove" && this.PageContext.ForumVoteAccess)
            {
               // this.ChangePollShowStatus(false);

                if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
                {
                    DB.poll_remove(this.PollGroupId, e.CommandArgument,this.BoardId, false,false);
                    ReturnToPage();
                    // this.BindData();
                }

            }
            if (e.CommandName == "removeall" && this.PageContext.ForumVoteAccess)
            {
                if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
                {
                    DB.poll_remove(this.PollGroupId, e.CommandArgument, this.BoardId, true, false);
                    ReturnToPage();
                    // this.BindData();
                }
            }
    
            if (e.CommandName == "removegroup" && this.PageContext.ForumVoteAccess)
            {
              
                    DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, false, false);
                    ReturnToPage();
                    // this.BindData();
              
            }
            if (e.CommandName == "removegroupall" && this.PageContext.ForumVoteAccess)
            {
             
                    DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, true, false);
                    ReturnToPage();
                    //this.BindData();
              
            }
            if (e.CommandName == "removegroupevery" && this.PageContext.ForumVoteAccess)
            {

                    DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, false, true);
                    ReturnToPage();
                // this.BindData();

            }
          
          

        }



        protected void Poll_OnItemDataBound(object source, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {

            RepeaterItem item = e.Item;
           
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                
                MyLinkButton myLinkButton = item.FindControlRecursiveAs<MyLinkButton>("MyLinkButton1");
                string pollId = item.FindControlRecursiveAs<HiddenField>("PollIDChoice").Value;

                bool isNotVoted = IsNotVoted(pollId);
                bool canVote = CanVote(pollId);

                myLinkButton.Enabled = canVote;
                myLinkButton.Visible = true;

                if (isBound)
                {
                    int voteCount = 0;
                    foreach (DataRow dr in _dtPollGroup.Rows)
                    {
                        if (!IsNotVoted(dr["PollID"]) && !IsPollClosed(dr["PollID"]))
                        {
                            voteCount++;
                        }
                        
                    }
                    if (!IsPollClosed(pollId) && voteCount >= this.PollNumber)
                   {
                       item.FindControlRecursiveAs<Panel>("resultsSpan").Visible =
                       item.FindControlRecursiveAs<Panel>("VoteSpan").Visible = true;
                   }
                }
                else
                {
                    if (isNotVoted || this.PageContext.BoardSettings.AllowUsersViewPollVotesBefore)
                    {
                       item.FindControlRecursiveAs<Panel>("resultsSpan").Visible =
                       item.FindControlRecursiveAs<Panel>("VoteSpan").Visible = true;
                    }
                }
               
            }
           
        }


        /// <summary>
        /// The poll_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
       
        protected void Poll_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "vote" && this.PageContext.ForumVoteAccess)
            {
                if (!this.CanVote(Convert.ToInt32(e.CommandArgument)))
                {
                    this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("WARN_ALREADY_VOTED"));
                    return;
                }

                if (this.IsLocked)
                {
                    this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("WARN_TOPIC_LOCKED"));
                    return;
                }

                if (this.IsPollClosed(Convert.ToInt32(e.CommandArgument)))
                {
                    this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("WARN_POLL_CLOSED"));
                    return;
                }

                object userID = null;
                object remoteIP = null;

                if (this.PageContext.BoardSettings.PollVoteTiedToIP)
                {
                    remoteIP = IPHelper.IPStrToLong(this.Request.ServerVariables["REMOTE_ADDR"]).ToString();
                }

                if (!this.PageContext.IsGuest)
                { 
                    userID = this.PageContext.PageUserID;
                }

                DB.choice_vote(e.CommandArgument, userID, remoteIP);

                // save the voting cookie...
                var c = new HttpCookie(VotingCookieName(Convert.ToInt32(e.CommandArgument)), e.CommandArgument.ToString());
                c.Expires = DateTime.UtcNow.AddYears(1);
                this.Response.Cookies.Add(c);
                string msg = this.PageContext.Localization.GetText("INFO_VOTED");

                if (this.isBound && this.PollNumber > 1 && this.PollNumber >= this._dtVotes.Rows.Count && (!this.PageContext.BoardSettings.AllowUsersViewPollVotesBefore))
                {
                    msg += this.PageContext.Localization.GetText("POLLGROUP_BOUNDWARN");
                }

                this.PageContext.AddLoadMessage(msg);
                this.BindData();
            }
        }

        private void ReturnToPage()
        {
           
            if (this.TopicId > 0)
            {
                YafBuildLink.Redirect(
                          ForumPages.posts,
                          "t={0}",
                         this.TopicId);
            }
            
            if (this.EditMessageId > 0)
            {
                YafBuildLink.Redirect(
                    ForumPages.postmessage,
                    "m={0}",
                    this.EditMessageId);
            }

            if (this.ForumId > 0)
            {
                YafBuildLink.Redirect(
                    ForumPages.topics,
                    "f={0}",
                    this.ForumId);
            }
            
            if (this.EditForumId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.admin_editforum,
                    "f={0}",
                    this.ForumId);
            }
            
            if (this.CategoryId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.forum,
                    "c={0}",
                    this.CategoryId);
            }
            
            if (this.EditCategoryId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.admin_editcategory,
                    "c={0}",
                    this.EditCategoryId);
            }
            
            if (this.BoardId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.forum);
            }
            
            if (this.EditBoardId > 0)
            {

               YafBuildLink.Redirect(
                    ForumPages.admin_editboard,
                    "b={0}",
                    this.EditBoardId);
            }
         
                YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
           
        }

        /// <summary>
        /// The change poll show status.
        /// </summary>
        /// <param name="newStatus">
        /// The new status.
        /// </param>
        protected void ChangePollShowStatus(bool newStatus)
        {
          /*  var pollRow = (HtmlTableRow)this.FindControl(String.Format("PollRow{0}", 1));

            if (pollRow != null)
            {
                pollRow.Visible = newStatus;
            }*/
        }

        protected bool CanCreatePoll()
        {
            return (this.PollNumber < this.PageContext.BoardSettings.AllowedPollNumber) && 
                (PageContext.BoardSettings.AllowedPollChoiceNumber > 0)  && 
                HasOwnerExistingGroupAccess() 
                && (this.PollGroupId >= 0);
        }
        protected bool CanEditPoll(object pollId)
        {
            if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return this.ShowButtons &&
                       (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                        (this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                         PollHasNoVotes(pollId) || (!IsPollClosed(pollId))));
            }
            else
            {
                return this.ShowButtons &&
                     (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                      this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) && (!IsPollClosed(pollId)));
            }
        }
        protected bool CanRemovePoll(object pollId)
        {
            return this.ShowButtons && 
                (this.PageContext.IsAdmin || 
                this.PageContext.IsForumModerator ||
                (this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"])));
        }
        protected bool CanRemovePollCompletely(object pollId)
        {
            if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return this.ShowButtons &&
                      (this.PageContext.IsAdmin || this.PageContext.IsModerator ||
                      ((this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                        PollHasNoVotes(pollId))));
            }
            else
            {
                return PollHasNoVotes(pollId) && this.ShowButtons &&
                        (this.PageContext.IsAdmin ||
                         this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]));  
            }
        }
        protected bool CanRemoveGroup()
        {
            bool hasNoVotes = true;
            foreach (DataRow dr in _dtPoll.Rows)
            {
                if (Convert.ToInt32(dr["Votes"]) > 0)
                {
                    hasNoVotes = false;
                }
            }

            if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return this.ShowButtons &&
                       (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                        ((this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                          hasNoVotes)));
            }
            else
            {
                return this.ShowButtons &&
                      (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                       ((this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]))));
            }
        }
        protected bool CanRemoveGroupCompletely()
        {
            bool hasNoVotes = true;
            foreach (DataRow dr in _dtPoll.Rows)
            {
                if (Convert.ToInt32(dr["Votes"]) > 0)
                {
                    hasNoVotes = false;
                }
            }

           
            if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return this.ShowButtons &&
                      (this.PageContext.IsAdmin || 
                       ((this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                        hasNoVotes)));
            }
            else
            {
                return this.ShowButtons &&
                        (this.PageContext.IsAdmin ||
                         this.PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]));
            }
           
        }
        protected bool CanRemoveGroupEverywhere()
        {
            return this.ShowButtons && (this.PageContext.IsAdmin);
        }

        private bool PollHasNoVotes(object pollId)
        {
                foreach (DataRow dr in _dtPoll.Rows)
                {
                    if (Convert.ToInt32(dr["PollID"]) == Convert.ToInt32(pollId))
                    {
                       if (Convert.ToInt32(dr["Votes"]) > 0)
                       {
                           return false;
                       }
                    }
                    
                }
            return true;

        }
   
        /// <summary>
        /// The remove poll_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemovePoll_Load(object sender, EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] = String.Format(
              "return confirm('{0}');", this.PageContext.Localization.GetText("POLLEDIT","ASK_POLL_DELETE"));
        }

        /// <summary>
        /// The remove poll_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemovePollCompletely_Load(object sender, EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] = String.Format(
              "return confirm('{0}');", this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
        }

       /* protected bool CanSeeResults()
        {
            if (this.PageContext.IsGuest)
            {
                return this.PageContext.BoardSettings.AllowUsersViewPollVotesBefore;
                
            }
            return true;
        } */

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {   
                this._dataBound = true;
                
                PollNumber = 0;
                DataRow drpg;
                _dtPoll = DB.pollgroup_stats(this.PollGroupId);

   
            _canChange = (Convert.ToInt32(_dtPoll.Rows[0]["GroupUserID"]) == this.PageContext.PageUserID) ||
                       PageContext.IsAdmin || PageContext.IsForumModerator;

            this.PollGroup.Visible = true;
           // _canChange || (((this.EditMessageId > 0)(this. > 0)) && (!_canChange));

            if (Parent.Page.ClientQueryString.Contains("postmessage"))
            {
                this.PollGroup.Visible = (((this.EditMessageId > 0)) && (!_canChange)) || _canChange;
            }
             
                int i = 0;
                 _dtPollGroup = _dtPoll.Copy();
                 foreach (DataRow drp in _dtPollGroup.Rows)
                {
                    if (i != Convert.ToInt32(drp["PollID"]))
                    {
                        PollNumber++;
                        i = (int) drp["PollID"];
                    }
                    else
                    {
                        drp.Delete();
                    }
                }
          
                _dtPollGroup.AcceptChanges();

                if (_dtPollGroup.Rows.Count > 0)
                {
                   
                    // Check if the user is already voted in polls in the group 
                    object userId = null;
                    object remoteIp = null;

                   if (this.PageContext.BoardSettings.PollVoteTiedToIP)
                   {
                       remoteIp = IPHelper.IPStrToLong(this.Request.UserHostAddress).ToString();
                   }

                    if (!this.PageContext.IsGuest)
                    {
                        userId = this.PageContext.PageUserID;
                    }

                    this._dtVotes = DB.pollgroup_votecheck(this.PollGroupId, userId, remoteIp);

                    this.isBound = Convert.ToInt32(_dtPollGroup.Rows[0]["IsBound"]) == 2;

                    this.PollGroup.DataSource = _dtPollGroup;
                    // this.PollGroup.DataBind();

                    // we hide new poll row if polls exist
                    this.NewPollRow.Visible = false;
                    ChangePollShowStatus(true);

                }
            
                

                //  this.ShowButtons = (Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]) == this.PageContext.PageUserID) || this.PageContext.IsModerator ||
                //     this.PageContext.IsAdmin;

                this.DataBind();
        }
    }
}