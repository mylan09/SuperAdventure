﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LivingCreature : INotifyPropertyChanged
    {
        public int MaximumHitPoints { get; set; }

        private int _currentHitPoints;
        public int CurrentHitPoints
        {
            get { return _currentHitPoints; }
            set
            {
                _currentHitPoints = value;
                OnPropertyChanged("CurrentHitPoints");
            }
        }

        public bool IsDead { get { return CurrentHitPoints <= 0; } }

        public LivingCreature(int currentHitPoints, int maximumHitPoints) 
        { 
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
        }

        //The PropertyChanged event is what the UI will “subscribe” to.
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(name));

            }
        }
    }

}
