using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digitalSignage
{
    class delegates
    {
        // need delegate to modify wpf gui from different thread
        // rss Delegates
        public delegate void updateNewsHeadlineCallback(string stringNewsHeadline);
        public delegate void updateNewsAbstractCallback(string stringNewsAbstract);
        public delegate void updateNewsImageCallback(string stringNewsImage);
        // Timetable delegates
        public delegate void updateYr11FirstLineCallback(string stringYr11firstline);
        public delegate void updateYr11SecondLineCallback(string stringYr11secondline);
        public delegate void updateYr12FirstLineCallback(string stringYr12firstline);
        public delegate void updateYr12SecondLineCallback(string stringYr12secondline);
        public delegate void updateYr11FirstLineActiveCallback(double doubleYr11size);
        public delegate void updateYr11SecondLineActiveCallback(double doubleYr11size);
        public delegate void updateYr12FirstLineActiveCallback(double doubleYr12size);
        public delegate void updateYr12SecondLineActiveCallback(double doubleYr12size);
        // Notices delegates
        public delegate void updateClassChangesCallback(string stringClassChanges);
        public delegate void updateMeetingChangesCallback(string stringMeetingChanges);
        public delegate void updateSpecEventsCallback(string stringSpecEvents);
    }
}
