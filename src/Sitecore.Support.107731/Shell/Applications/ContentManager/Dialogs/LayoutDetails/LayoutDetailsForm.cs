namespace Sitecore.Support.Shell.Applications.ContentManager.Dialogs.LayoutDetails
{
    using Sitecore.Data.Fields;
    using Sitecore.Diagnostics;
    using Sitecore.Xml.Patch;

    /// <summary>
    /// Represents a gallery layout form.
    /// </summary>
    public class LayoutDetailsForm : Sitecore.Shell.Applications.ContentManager.Dialogs.LayoutDetails.LayoutDetailsForm
    {
        /// <summary>
        /// Gets or sets the final layout.
        /// </summary>
        /// <value>The final layout.</value>
        public override string FinalLayout
        {
            get
            {
                string layoutDelta = this.LayoutDelta;
                if (string.IsNullOrWhiteSpace(layoutDelta))
                {
                    return this.Layout;
                }
                if (XmlPatchUtils.IsXmlPatch(layoutDelta))
                {
                    return XmlDeltas.ApplyDelta(this.Layout, layoutDelta);
                }
                return layoutDelta;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                if (!string.IsNullOrWhiteSpace(this.Layout))
                {
                    this.LayoutDelta = XmlDeltas.GetDelta(value, this.Layout);
                    return;
                }
                this.LayoutDelta = value;
            }
        }
    }
}
