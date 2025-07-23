using ITravelApp.Data.Data;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITravelApp.Data
{
    public class AdminDAO
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly itravel_client_dbContext _db;

        public AdminDAO(itravel_client_dbContext db, IStringLocalizer<Messages> localizer)
        {
            _db = db;
            _localizer = localizer;
        }
        #region destination

        //save main destination data by admin
        public ResponseCls SaveMainDestination(destination_main row)
        {
            ResponseCls response;
            int maxId = 0;
            var msg = _localizer["DuplicateData"];
            try
            {
               
                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_mains.Where(wr => wr.dest_code == row.dest_code && wr.active == row.active).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_mains.Count() > 0)
                    {
                        maxId = _db.destination_mains.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_mains.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.destination_mains.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true,idOut=row.id };


            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false,idOut=0 };
            }

            return response;
        }
        //save destination's translations
        public ResponseCls SaveDestinationTranslations(destination_translation row)
        {
            ResponseCls response;
            int maxId = 0;
            try
            {

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_translations.Where(wr => wr.dest_name == row.dest_name && wr.destination_id == row.destination_id && wr.active == row.active && wr.lang_code == row.lang_code).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_translations.Count() > 0)
                    {
                        maxId = _db.destination_translations.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_translations.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.destination_translations.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };


            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }

            return response;
        }

        public ResponseCls saveDestinationImage(destination_img row)
        {
            ResponseCls response;
            int maxId = 0;
            try
            {

                if (row.id == 0)
                {
                    //check duplicate validation
                    var result = _db.destination_imgs.Where(wr => wr.destination_id == row.destination_id && wr.is_default == row.is_default).SingleOrDefault();
                    if (result != null)
                    {
                        return new ResponseCls { success = false, errors = _localizer["DuplicateData"] };
                    }
                    if (_db.destination_imgs.Count() > 0)
                    {
                        maxId = _db.destination_imgs.Max(d => d.id);

                    }
                    row.id = maxId + 1;
                    _db.destination_imgs.Add(row);
                    _db.SaveChanges();
                }
                else
                {
                    _db.destination_imgs.Update(row);
                    _db.SaveChanges();
                }

                response = new ResponseCls { errors = null, success = true, idOut = row.id };


            }

            catch (Exception ex)
            {
                response = new ResponseCls { errors = _localizer["CheckAdmin"], success = false, idOut = 0 };
            }
            return response;
        }
        #endregion
    }
}
