using Backload.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Backload.Bundles
{

    /// <summary>
    /// Registers bundles for client side scripts and styles. This is an optional feature.
    /// </summary>
    public class BackloadBundles
    {

        /// <summary>
        /// Registers bundles for the client side scripts and styles
        /// </summary>
        /// <param name="bundles">A BundleCollection instance</param>
        /// <remarks>This is optional. The Backload component does not need bundeling internally.</remarks>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Default path to the client side files
            string vendor = "blueimp";
            string plugin = "fileupload";
            string jsroot = "~/Backload/Client";
            string cssroot = "~/Backload/Client";
            string jsvendor = string.Empty;
            string cssvendor = string.Empty;
            string jsplugin = string.Empty;
            string cssplugin = string.Empty;


            // Note: Comment this section in, if you use backload with a different (not default) path to the client files
            #region Get the path to the client side scripts and styles from the configuration file
            try
            {
                Backload.Configuration.Bundles bundle = new Backload.Configuration.Bundles();
                jsroot = Backload.Configuration.Bundles.ClientScripts;
                cssroot = Backload.Configuration.Bundles.ClientStyles;
            }
            catch
            {
            }
            #endregion



            
            #region Bundles for the jQuery File Upload Plugin

            // Bundle registration starts here
            jsvendor = string.Format("{0}/{1}/", jsroot, vendor);
            jsplugin = string.Format("{0}{1}/js/", jsvendor, plugin);
            cssvendor = string.Format("{0}/{1}/", cssroot, vendor);
            cssplugin = string.Format("{0}{1}/css/", cssvendor, plugin);

 
            
            #region jQuery File Upload Plugin: Basic theme (Bootstrap)

            string[] scripts = new string[] {
                jsplugin + "vendor/jquery.ui.widget.js",
                jsplugin + "jquery.iframe-transport.js",
                jsplugin + "jquery.fileupload.js",
                jsplugin + "themes/jquery.fileupload-themes.js" };
            
            string[] styles = new string[] {
                cssplugin + "jquery.fileupload.css" };
            

            bundles.Add(new ScriptBundle("~/backload/blueimp/bootstrap/Basic").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/blueimp/bootstrap/Basic/css").Include(styles));

            // The following virtual path is for backward compatibility only and can be removed
            bundles.Add(new ScriptBundle("~/bundles/fileUpload/bootstrap/Basic/js").Include(scripts));
            bundles.Add(new StyleBundle("~/bundles/fileUpload/bootstrap/Basic/css").Include(styles));
            
            #endregion



            #region jQuery File Upload Plugin: Basic Plus (Bootstrap)

            scripts = new string[] {
                jsvendor + "loadimage/js/load-image.all.min.js",
                jsvendor + "blob/js/canvas-to-blob.min.js",
                jsplugin + "vendor/jquery.ui.widget.js",
                jsplugin + "jquery.iframe-transport.js",
                jsplugin + "jquery.fileupload.js",
                jsplugin + "jquery.fileupload-process.js",
                jsplugin + "jquery.fileupload-image.js",
                jsplugin + "jquery.fileupload-audio.js",
                jsplugin + "jquery.fileupload-video.js",
                jsplugin + "jquery.fileupload-validate.js",
                jsplugin + "themes/jquery.fileupload-themes.js" };

            styles = new string[] {
                cssplugin + "jquery.fileupload.css" };


            bundles.Add(new ScriptBundle("~/backload/blueimp/bootstrap/BasicPlus").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/blueimp/bootstrap/BasicPlus/css").Include(styles));

            // The following virtual path is for backward compatibility only and can be removed
            bundles.Add(new ScriptBundle("~/bundles/fileUpload/bootstrap/BasicPlus/js").Include(scripts));
            bundles.Add(new StyleBundle("~/bundles/fileUpload/bootstrap/BasicPlus/css").Include(styles));
            
            #endregion



            #region jQuery File Upload Plugin: Basic Plus UI (Bootstrap)

            scripts = new string[] {
                jsvendor + "templates/js/tmpl.min.js",
                jsvendor + "loadimage/js/load-image.all.min.js",
                jsvendor + "blob/js/canvas-to-blob.min.js",
                jsvendor + "gallery/js/jquery.blueimp-gallery.min.js",
                jsplugin + "vendor/jquery.ui.widget.js",
                jsplugin + "jquery.iframe-transport.js",
                jsplugin + "jquery.fileupload.js",
                jsplugin + "jquery.fileupload-process.js",
                jsplugin + "jquery.fileupload-image.js",
                jsplugin + "jquery.fileupload-audio.js",
                jsplugin + "jquery.fileupload-video.js",
                jsplugin + "jquery.fileupload-validate.js",
                jsplugin + "jquery.fileupload-ui.js",
                jsplugin + "themes/jquery.fileupload-themes.js" };

            styles = new string[] {
                cssvendor + "gallery/css/blueimp-gallery.min.css",
                cssplugin + "jquery.fileupload.css",
                cssplugin + "jquery.fileupload-ui.css" };


            bundles.Add(new ScriptBundle("~/backload/blueimp/bootstrap/BasicPlusUI").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/blueimp/bootstrap/BasicPlusUI/css").Include(styles));

            // The following virtual path is for backward compatibility only and can be removed
            bundles.Add(new ScriptBundle("~/bundles/fileUpload/bootstrap/BasicPlusUI/js").Include(scripts));
            bundles.Add(new StyleBundle("~/bundles/fileUpload/bootstrap/BasicPlusUI/css").Include(styles));
            
            #endregion



            #region jQuery File Upload Plugin: AngularJS theme

            scripts = new string[] {
                jsvendor + "loadimage/js/load-image.all.min.js",
                jsvendor + "blob/js/canvas-to-blob.min.js",
                jsvendor + "gallery/js/jquery.blueimp-gallery.min.js",
                jsplugin + "vendor/jquery.ui.widget.js",
                jsplugin + "jquery.iframe-transport.js",
                jsplugin + "jquery.fileupload.js",
                jsplugin + "jquery.fileupload-process.js",
                jsplugin + "jquery.fileupload-image.js",
                jsplugin + "jquery.fileupload-audio.js",
                jsplugin + "jquery.fileupload-video.js",
                jsplugin + "jquery.fileupload-validate.js",
                jsplugin + "jquery.fileupload-angular.js" };

            styles = new string[] {
                cssvendor + "gallery/css/blueimp-gallery.min.css",
                cssplugin + "jquery.fileupload.css",
                cssplugin + "jquery.fileupload-ui.css" };


            bundles.Add(new ScriptBundle("~/backload/blueimp/angularjs").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/blueimp/angularjs/css").Include(styles));

            // The following virtual path is for backward compatibility only and can be removed
            bundles.Add(new ScriptBundle("~/bundles/fileUpload/angularjs/js").Include(scripts));
            bundles.Add(new StyleBundle("~/bundles/fileUpload/angularjs/css").Include(styles));
            
            #endregion



            #region jQuery File Upload Plugin: jQueryUI theme

            scripts = new string[] {
                jsvendor + "templates/js/tmpl.min.js",
                jsvendor + "loadimage/js/load-image.all.min.js",
                jsvendor + "blob/js/canvas-to-blob.min.js",
                jsvendor + "gallery/js/jquery.blueimp-gallery.min.js",
                jsplugin + "jquery.iframe-transport.js",
                jsplugin + "jquery.fileupload.js",
                jsplugin + "jquery.fileupload-process.js",
                jsplugin + "jquery.fileupload-image.js",
                jsplugin + "jquery.fileupload-audio.js",
                jsplugin + "jquery.fileupload-video.js",
                jsplugin + "jquery.fileupload-validate.js",
                jsplugin + "jquery.fileupload-ui.js",
                jsplugin + "jquery.fileupload-jquery-ui.js" };

            styles = new string[] {
                cssvendor + "gallery/css/blueimp-gallery.min.css",
                cssplugin + "jquery.fileupload.css",
                cssplugin + "jquery.fileupload-ui.css" };


            bundles.Add(new ScriptBundle("~/backload/blueimp/jqueryui").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/blueimp/jqueryui/css").Include(styles));

            // The following virtual path is for backward compatibility only and can be removed
            bundles.Add(new ScriptBundle("~/bundles/fileupload/jqueryui/BasicPlusUI/js").Include(scripts));
            bundles.Add(new StyleBundle("~/bundles/fileupload/jqueryui/BasicPlusUI/css").Include(styles));
            
            #endregion            
            
            #endregion
 
  

         
            #region Fine Uploader

            vendor = "widen";
            plugin = "fineuploader";

            // Fine Uploader from Widen Enterprises
            jsvendor = string.Format("{0}/{1}/", jsroot, vendor);
            jsplugin = string.Format("{0}{1}/js/", jsvendor, plugin);
            cssvendor = string.Format("{0}/{1}/", cssroot, vendor);
            cssplugin = string.Format("{0}{1}/", cssvendor, plugin);


            // Simple and default theme
            scripts = new string[] {
                jsplugin + "fine-uploader.min.js" };
            
            styles = new string[] {
                cssplugin + "fine-uploader-new.min.css" };

            bundles.Add(new ScriptBundle("~/backload/widen/fineuploader/simple").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/widen/fineuploader/simple/css").Include(styles));


            // Gallery theme
            styles = new string[] {
                cssplugin + "fine-uploader-gallery.min.css",
                cssplugin + "fine-uploader-new.min.css" };

            bundles.Add(new ScriptBundle("~/backload/widen/fineuploader/gallery").Include(scripts));
            bundles.Add(new StyleBundle("~/backload/widen/fineuploader/gallery/css").Include(styles));
            
            #endregion            


           
         
            #region PlUpload

            vendor = "moxie";
            plugin = "plupload";

            // Fine Uploader from Widen Enterprises
            jsvendor = string.Format("{0}/{1}/", jsroot, vendor);
            jsplugin = string.Format("{0}{1}/js/", jsvendor, plugin);
            cssplugin = jsplugin;


            // Simple theme
            scripts = new string[] {
                jsplugin + "plupload.full.min.js" };
            
            bundles.Add(new ScriptBundle("~/backload/moxie/plupload/simple").Include(scripts));


            // UI theme. We need to order the files, otherwise System.Web.Optimization produces a false order
            scripts = new string[] {
                jsplugin + "plupload.full.min.js",
                jsplugin + "jquery.ui.plupload/jquery.ui.plupload.min.js" };

            ScriptBundle scriptBundle = (ScriptBundle)new ScriptBundle("~/backload/moxie/plupload/ui").Include(scripts);
            BackloadBundleOrderer orderer = new BackloadBundleOrderer();
            scriptBundle.Orderer = orderer;

            styles = new string[] {
                cssplugin + "jquery.ui.plupload/css/jquery.ui.plupload.css" };

            bundles.Add(scriptBundle);
            bundles.Add(new StyleBundle("~/backload/moxie/plupload/ui/css").Include(styles));
            
            #endregion            
            
        }



        #region BackloadBundleOrderer

        /// <summary>
        /// Orders the scripts in the scripts like we added them.
        /// </summary>
        private class BackloadBundleOrderer : IBundleOrderer
        {

            /// <summary>
            /// Old IBundleOrderer interface
            /// </summary>
            /// <param name="context">BundleContext</param>
            /// <param name="files">IEnumerable</param>
            /// <returns>Ordered files</returns>
            public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files)
            {
                return files;
            }


            /// <summary>
            /// New IBundleOrderer interface
            /// </summary>
            /// <param name="context">BundleContext</param>
            /// <param name="files">IEnumerable</param>
            /// <returns>Ordered files</returns>
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }

        #endregion
    }
}
