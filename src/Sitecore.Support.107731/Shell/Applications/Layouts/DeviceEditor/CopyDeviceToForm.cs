namespace Sitecore.Support.Shell.Applications.Layouts.DeviceEditor
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.UI;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework;
    using Sitecore.Text;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.Web.UI.WebControls;


    /// <summary>
    /// Represents a Copy To Form.
    /// </summary>
    public class CopyDeviceToForm : DialogForm
    {
        #region Fields

        /// <summary></summary>
        protected DataContext DataContext;

        /// <summary></summary>
        protected TreeviewEx Treeview;
        /// <summary></summary>
        protected Border Devices;

        #endregion

        #region Public methods

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            Dispatcher.Dispatch(message, GetCurrentItem(message));

            base.HandleMessage(message);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnLoad(e);

            if (Context.ClientPage.IsEvent)
            {
                return;
            }

            DataContext.GetFromQueryString();

            RenderDevices();

        }

        /// <summary>
        /// Renders the devices.
        /// </summary>
        void RenderDevices()
        {
            ListString selected = new ListString(Registry.GetValue("/Current_User/DeviceEditor/CopyDevices/TargetDevices"));

            Item devices = Client.GetItemNotNull(ItemIDs.DevicesRoot);

            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());

            foreach (Item device in devices.Children)
            {
                string id = "de_" + device.ID.ToShortID();

                output.Write("<div style=\"padding:2px\">");
                output.Write("<input type=\"checkbox\" id=\"" + id + "\" name=\"" + id + "\"");

                if (selected.Contains(device.ID.ToString()))
                {
                    output.Write(" checked=\"checked\"");
                }

                output.Write(" />");
                output.Write("<label for=\"" + id + "\">");
                output.Write(device.DisplayName);
                output.Write("</label>");
                output.Write("</div>");
            }

            Devices.InnerHtml = output.InnerWriter.ToString();
        }

        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <remarks>When the user clicks OK, the dialog is closed by calling
        /// the <see cref="Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.</remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            Item item = Treeview.GetSelectionItem();

            if (item == null)
            {
                SheerResponse.Alert(Texts.PLEASE_SELECT_AN_ITEM);
            }

            if (item == null)
            {
                SheerResponse.Alert(Texts.THE_TARGET_ITEM_COULD_NOT_BE_FOUND);
                return;
            }

            ListString devices = new ListString();

            foreach (string key in HttpContext.Current.Request.Form.Keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                if (key.StartsWith("de_", StringComparison.InvariantCulture))
                {
                    devices.Add(ShortID.Decode(StringUtil.Mid(key, 3)));
                }
            }

            if (devices.Count == 0)
            {
                SheerResponse.Alert("Please select one or more devices.");
                return;
            }

            Registry.SetValue("/Current_User/DeviceEditor/CopyDevices/TargetDevices", devices.ToString());

            SheerResponse.SetDialogValue(devices + "^" + item.ID);

            base.OnOK(sender, args);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [CanBeNull]
        Item GetCurrentItem(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            string id = message["id"];

            Item folder = DataContext.GetFolder();

            Language language = Context.Language;
            if (folder != null)
            {
                language = folder.Language;
            }

            if (!string.IsNullOrEmpty(id))
            {
                return Client.ContentDatabase.GetItem(id, language);
            }

            return folder;
        }

        #endregion
    }
}