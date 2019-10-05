using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BikeSharingSimulation
{
    public partial class Form1 : Form
    {

        
        public Form1()
        {
            InitializeComponent();
            rnd2.Lambda = 40;

            
            List<Station> StationList = new List<Station>();
            List<FutureEvent> FutureEventList = new List<FutureEvent>();

            // For each station in the list creat events and add new events to the FutureEventList
            foreach (Station s in StationList) 
            {
                double GlobalTime = 0;
                Troschuetz.Random.Distributions.Discrete.PoissonDistribution rnd = new Troschuetz.Random.Distributions.Discrete.PoissonDistribution();

                for (int t = 0; t < 24; t++)
                {
                    GlobalTime += rnd.NextDouble();

                    //?? how mix request matrix with time ?
                    FutureEventList.Add(new FutureEvent(s,t, EventType.CustomerRequest )); // ??? how add location of user ?
                }    
            }
            
            
            FutureEventList.Sort();

            for (int i = 0; i < FutureEventList.Count; i++)
            {
                if (FutureEventList[i].EventType == EventType.CustomerRequest)
                {
                    FutureEvent e = new FutureEvent();
                    e.Station = FutureEventList[i].Station;
                    e.StartTime = FutureEventList[i].StartTime;

                    if (FutureEventList[i].Station.AvailebleBikes > 0)
                        e.EventType = EventType.BikeRentStart;
                    else
                        e.EventType = EventType.MissRequest;
                    

                    FutureEventList.Add(e);

                }
                else if (FutureEventList[i].EventType == EventType.BikeRentStart)
                {
                    FutureEventList[i].Station.AvailebleBikes -= 1;

                    FutureEvent e = new FutureEvent();
                    e.EventType = EventType.BikeRentFinish;

                    e.Station = new Station(); // #### Next Station Should be determined in here 
                    e.StartTime = DateTime.Now;  // #### distance and duration to next station Should be determined in here 
                    e.Distance = 0;  // #### distance and duration to next station Should be determined in here 

                    FutureEventList.Add(e);

                    if (FutureEventList[i].Station.AvailebleBikes <= 0)
                    {
                        FutureEvent re = new FutureEvent();
                        re.EventType = EventType.RebalancingRequest;
                        re.Station = FutureEventList[i].Station;
                        re.StartTime = FutureEventList[i].StartTime;
                        FutureEventList.Add(re);
                    }

                }
                else if (FutureEventList[i].EventType == EventType.BikeRentFinish)
                {
                    FutureEventList[i].Station.AvailebleBikes += 1;

                    if (FutureEventList[i].Station.AvailebleBikes >= FutureEventList[i].Station.Capasity)
                    {
                        FutureEvent re = new FutureEvent();
                        re.EventType = EventType.RebalancingRequest;
                        re.Station = FutureEventList[i].Station;
                        re.StartTime = FutureEventList[i].StartTime;
                        FutureEventList.Add(re);
                    }

                }
                else if (FutureEventList[i].EventType == EventType.RebalancingStart)
                {
                    //?? Reblancing To which stations should be done? one station or multiple station ?
                    //e.StartTime = DateTime.Now;  // #### distance and duration to next station Should be determined in here 
                    //e.Distance = 0;  // #### distance and duration to next station Should be determined in here 

                    FutureEvent e = new FutureEvent();
                    e.EventType = EventType.RebalancingEnd;
                    int duration = 0;
                    float distance = 0;

                    e.Distance = distance;
                    e.Duration = duration;
                    e.StartTime = FutureEventList[i].StartTime.AddTicks(duration);
                    FutureEventList.Add(e);
                }
                if (FutureEventList[i].EventType == EventType.RebalancingEnd)
                {
                    FutureEventList[i].Station.AvailebleBikes += 1; // ?? How many bikes should be added to the station ?
                    FutureEventList[i].Duration = 0;
                    FutureEventList[i].Distance = 0;

                }

            }

        }

        Troschuetz.Random.Distributions.Discrete.PoissonDistribution rnd2 = new Troschuetz.Random.Distributions.Discrete.PoissonDistribution();
        private void button1_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show(rnd2.Next().ToString());  
        }
    }

    public enum EventType
    {
        CustomerRequest,
        MissRequest,
        BikeRentStart,
        BikeRentFinish,
        RebalancingRequest,
        RebalancingStart,
        RebalancingEnd
    }

    class FutureEvent
    {
        public FutureEvent()
        {
        }

        public FutureEvent(Station s,double time,EventType e)
        {
            Station = s;
            //StartTime = time;
            EventType = e;
        }

        public Station Station;
        public DateTime StartTime;
        public EventType EventType;

        public int Duration;
        public float Distance;
        public int NumberOfBikesForRebalancing;

    }

    class Station
    {
        public string Name;
        public float Weight;
        public float Lat;
        public float Long;
        public int AvailebleBikes;
        public int Capasity;
    }
}
