﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Engine2.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Engine2.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die {&quot;producer&quot;:{&quot;name&quot;:&quot;Blender&quot;,&quot;version&quot;:&quot;2.78 (sub 0)&quot;,&quot;exporter_version&quot;:&quot;5.4.0&quot;,&quot;file&quot;:&quot;untitled.babylon&quot;},
        ///&quot;autoClear&quot;:true,&quot;clearColor&quot;:[0.0509,0.0509,0.0509],&quot;ambientColor&quot;:[0,0,0],&quot;gravity&quot;:[0,-9.81,0],
        ///&quot;materials&quot;:[],
        ///&quot;multiMaterials&quot;:[],
        ///&quot;skeletons&quot;:[],
        ///&quot;meshes&quot;:[{&quot;name&quot;:&quot;part 013&quot;,&quot;id&quot;:&quot;part 013&quot;,&quot;billboardMode&quot;:0,&quot;position&quot;:[-0.3733,0,0],&quot;rotation&quot;:[-1.5708,0,0],&quot;scaling&quot;:[1,1,1],&quot;isVisible&quot;:true,&quot;freezeWorldMatrix&quot;:false,&quot;isEnabled&quot;:true,&quot;checkCollisions&quot;:false,&quot;receiveShadows&quot;:false,&quot;tags&quot;: [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        internal static string Car {
            get {
                return ResourceManager.GetString("Car", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap SkyBox {
            get {
                object obj = ResourceManager.GetObject("SkyBox", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Skybox_example {
            get {
                object obj = ResourceManager.GetObject("Skybox_example", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap TILE {
            get {
                object obj = ResourceManager.GetObject("TILE", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}
