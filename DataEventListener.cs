using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Java.Lang;
using Boolean = Java.Lang.Boolean;

namespace Hydria_Calculator.Droid
{
    public class DataEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<DataEventArgs> DataRetrieved; 
        public class DataEventArgs : EventArgs
        {
            public float dataPoint { get; set; }
            
        }
    
        public void OnDataChange(DataSnapshot snapshot)
        {
            float num; 
            if (snapshot.Value != null)
            {
                num = (float)snapshot.GetValue(false);                
            }
            else
            {
                num = 0; 
            }
            DataRetrieved.Invoke(this, new DataEventArgs { dataPoint = num });
        }

        public void OnCancelled(DatabaseError error)
        {

        }

    }
}