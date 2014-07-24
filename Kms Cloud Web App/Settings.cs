using System;
using System.Collections.Generic;

namespace Kms.Cloud.WebApp.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        public List<Byte[]> AllowedUploadHeadersBytes {
            get {
                if ( mAllowedUploadHeadersBytes != null )
                    return mAllowedUploadHeadersBytes;

                mAllowedUploadHeadersBytes = new List<Byte[]>();

                foreach ( var bytesString in AllowedUploadHeadersHex ) {
                    if ( bytesString.Length < 2 || bytesString.Length % 2 != 0 )
                        throw new InvalidCastException(
                            "Allowed file upload byte header is invalid: " + bytesString
                        );

                    var byteChars = bytesString.ToCharArray();
                    var bytes     = new Byte[byteChars.Length / 2];

                    for ( int s = 0, i = 0, n = 1; s < bytes.Length; s++, i += 2, n += 2 )
                        bytes[s] = Convert.ToByte(byteChars[i] + byteChars[n]);

                    mAllowedUploadHeadersBytes.Add(bytes);
                }

                return mAllowedUploadHeadersBytes;
            }
        }
        private List<Byte[]> mAllowedUploadHeadersBytes;
    }
}
