using BlueCat.MySqlRepository.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlueCat.MySqlRepository
{
    public abstract class BaseRepository
    {
        protected BaseRepository(BaseContext context) //: base(context.ConnectionString)
        {

        }
    }
}
