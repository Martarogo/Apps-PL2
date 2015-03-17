using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocabulary
{
    public class Message
    {
        private int _sec;
        private int _n;
        private String _packet;

        public int num
        {
            get
            {
                return _n;
            }
        }

        public String packet
        {
            get
            {
                return _packet;
            }
        }

        public Message(int sec, int n)
        {
            _sec = sec;
            _n = n;
            _packet = _sec.ToString() + " | " + n.ToString();
        }

        public Message(int sec)
        {
            _sec = sec;
            _packet = _sec.ToString();
        }
    }
}
