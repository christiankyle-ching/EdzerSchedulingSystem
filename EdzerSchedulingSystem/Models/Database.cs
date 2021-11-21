using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EdzerSchedulingSystem.Models
{
    public static class Database
    {

        //SCHEDULES SQL
        public static int getQuantity(String type)
        {
            int id = -1;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT QUANTITY FROM `tbl_instrumentrented` WHERE `FK_InstrumentType_InstrumentType` = @TYPE";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        return id;
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return id;
        }



        public static void updateScheduleTypePrice(string type, float newPrice, string date)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_scheduletypeprice`( `ScheduleTypeID`, `ScheduleTypeName`, `PricePerHour`, `DateEffective`) VALUES  (@ID,@TYPE, @PRICE, @DATE)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@PRICE", newPrice);
            dbCommand.Parameters.AddWithValue("@DATE", date);
            dbCommand.Parameters.AddWithValue("@ID", Database.getScheduleTypeID(type));
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void addScheduleTypePrice(string type, float price)
        {
            //add its priceperhour in table
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            ;
            string query = "INSERT INTO `tbl_scheduletypeprice`(ScheduleTypeID,`ScheduleTypeName`, `PricePerHour`, `DateEffective`)";
            query += "VALUES((Select ScheduleTypeID FROM tbl_scheduletype WHERE ScheduleTypeName = @TYPE),@TYPE, @PRICE, @DATE)";
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@PRICE", price);
            dbCommand.Parameters.AddWithValue("@DATE", (DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00")); //add date effective default today at 12am
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader(); //execute insert into command

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

        }

        public static void addScheduleType(String type)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_scheduletype`(`ScheduleTypeName`) VALUES (@SchedType)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@SchedType", type.ToUpper());
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static float getScheduleTypePricePerHour(string type, string date)
        {
            float price = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT PricePerHour FROM `tbl_scheduletypeprice`";
            query += "WHERE `ScheduleTypeName`= @TYPE AND `DateEffective`<= @DATE ";
            query += "ORDER BY `DateEffective` DESC LIMIT 1";
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@DATE", DateTime.Parse(date).ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        price = reader.GetFloat(0);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);


            }

            return price;
        }

        public static int countScheduleOfMonth(DateTime date)
        {
            int scheduleCount = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT COUNT(*) FROM tbl_schedule WHERE MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR AND IsDeleted = 0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
            dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
            dbCommand.CommandTimeout = 60;
            
            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();

                //fetch count - returns 0 if nothing fetched
                scheduleCount = Convert.ToInt32(dbCommand.ExecuteScalar());
                
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return scheduleCount;
        }

        public static List<Schedule> getScheduleOfDay(DateTime date)
        {
            List<Schedule> listOfSchedule = new List<Schedule>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `ScheduleID`, tbl_band.BandName, tbl_scheduletype.ScheduleTypeName, `FK_Representative_RepresentativeName`, `ScheduleDate`, `StartTime`, `DurationInHours`, `IsPaid`, tbl_scheduletype.ScheduleTypeID FROM ";
            query += "((tbl_schedule INNER JOIN tbl_band on tbl_schedule.FK_Band_BandID = tbl_band.BandID) ";
            query += "INNER JOIN tbl_scheduletype on tbl_schedule.FK_ScheduleType_ScheduleTypeID = tbl_scheduletype.ScheduleTypeID) ";
            query += "WHERE `ScheduleDate` = @DATE AND `IsDeleted`=0 ";
            query += "ORDER BY `StartTime` ASC ";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@DATE", date.ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Schedule tmpSchedule = new Schedule();
                        tmpSchedule.scheduleID = reader.GetInt32(0);
                        tmpSchedule.bandName = reader.GetString(1);
                        tmpSchedule.scheduleType = reader.GetString(2);
                        tmpSchedule.representativeName = reader.GetString(3);
                        tmpSchedule.scheduleDate = reader.GetString(4);
                        tmpSchedule.startTime = DateTime.Parse(reader.GetString(5)).ToString("hh:mm tt");
                        tmpSchedule.duration = reader.GetInt32(6);
                        tmpSchedule.isPaid = reader.GetBoolean(7);
                        tmpSchedule.scheduleTypeID = reader.GetInt32(8);
                        tmpSchedule.studioRate = Database.getScheduleTypePrice(tmpSchedule.scheduleTypeID, tmpSchedule.scheduleDate);
                        
                        listOfSchedule.Add(tmpSchedule);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listOfSchedule;
        }

        public static Schedule getScheduleDetails(int scheduleID)
        {
            Schedule schedule = new Schedule();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `ScheduleID`, `FK_ScheduleType_ScheduleTypeID`, tbl_band.BandID, tbl_band.BandName, tbl_scheduletype.ScheduleTypeName, `FK_Representative_RepresentativeName`, `ScheduleDate`, `StartTime`, `DurationInHours`, `IsPaid`, tbl_band.HasPenalty FROM ";
            query += "((tbl_schedule INNER JOIN tbl_band on tbl_schedule.FK_Band_BandID = tbl_band.BandID) ";
            query += "INNER JOIN tbl_scheduletype on tbl_schedule.FK_ScheduleType_ScheduleTypeID = tbl_scheduletype.ScheduleTypeID) ";
            query += "WHERE `ScheduleID` = @ID ";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        schedule.scheduleID = reader.GetInt32(0);
                        schedule.scheduleTypeID = reader.GetInt32(1);
                        schedule.bandID = reader.GetInt32(2);
                        schedule.bandName = reader.GetString(3);
                        schedule.scheduleType = reader.GetString(4);
                        schedule.representativeName = reader.GetString(5);
                        schedule.scheduleDate = reader.GetString(6);
                        schedule.startTime = DateTime.Parse(reader.GetString(7)).ToString("hh:mm tt");
                        schedule.duration = reader.GetInt32(8);
                        schedule.isPaid = reader.GetBoolean(9);
                        schedule.studioRate = Database.getScheduleTypePrice(schedule.scheduleTypeID, schedule.scheduleDate);
                        schedule.hasPenalty = reader.GetBoolean(10);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return schedule;
        }

        public static List<string> getScheduleTypes()
        {
            List<string> scheduleTypes = new List<string>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `ScheduleTypeName` FROM `tbl_scheduletype` ORDER BY `ScheduleTypeName`";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        scheduleTypes.Add(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return scheduleTypes;
        }

        public static int getScheduleTypeID(string type)
        {
            int id = -1;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `ScheduleTypeID` FROM `tbl_scheduletype` WHERE `ScheduleTypeName`=@TYPE";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        return id;
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return id;
        }

//        public static List<InstrumentType> getInstrumentTypesRented(int scheduleID)
//        {
//            List<InstrumentType> listOfInstrumentTypeRented = new List<InstrumentType>();

//            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

//            string query = "SELECT `FK_InstrumentType_InstrumentType`, `FK_Schedule_ScheduleID`, COUNT(tbl_instrument.InstrumentID), SUM(DISTINCT `Quantity`), tbl_instrumenttype.PricePerHour ";
//            query += "FROM((tbl_instrumentrented INNER JOIN tbl_instrument ON tbl_instrumentrented.FK_InstrumentType_InstrumentType = tbl_instrument.InstrumentType) ";
//            query += "INNER JOIN tbl_instrumenttype ON tbl_instrumentrented.FK_InstrumentType_InstrumentType = tbl_instrumenttype.InstrumentType) ";
//            query += "WHERE tbl_instrumentrented.FK_Schedule_ScheduleID = @ID AND tbl_instrument.IsDeleted = 0 ";
//            query += "GROUP BY (tbl_instrument.InstrumentType)";

//            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
//            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
//            dbCommand.CommandTimeout = 60;

//            MySqlDataReader reader;

//            Console.WriteLine(query);

//            try
            
//{
//                //open and close connection
//                dbConnection.Open();
//                reader = dbCommand.ExecuteReader();

//                if (reader.HasRows)
//                {
//                    while (reader.Read())
//                    {
//                        //check first if null
//                        if (reader.GetString(0) == "NULL")
//                        {
//                            break;
//                        }

//                        listOfInstrumentTypeRented.Add(new InstrumentType
//                        {
//                            instrumentType = reader.GetString(0),
//                            scheduleID = reader.GetInt32(1),
//                            totalQuantity = Database.getTotalAvailableInstrument(reader.GetString(0)),
//                            rentedQuantity = reader.GetInt32(3),
//                            pricePerHour = Database.getInstrumentTypePricePerHour(reader.GetString(0), (Database.getScheduleDetails(scheduleID)).scheduleDate),
//                            totalPrice = (float) (reader.GetFloat(4) * reader.GetInt32(3))
//                        });
//                    }
//                }

//                dbConnection.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Database Error: " + ex.Message);
//            }
            
//            return listOfInstrumentTypeRented;
//        }

        public static int getTotalAvailableInstrument(string type)
        {
            int total = 0;
            
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT COUNT(`InstrumentID`) ";
            query += "FROM tbl_instrument ";
            query += "WHERE `InstrumentType`= @TYPE AND IsDeleted=0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        total = reader.GetInt32(0);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return total;
        }

        public static InstrumentType getInstrumentType(string type)
        {
            InstrumentType it = new InstrumentType();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "SELECT tbl_instrumenttype.InstrumentType, tbl_instrumenttype.PricePerHour, COUNT(tbl_instrument.InstrumentID) ";
            query += "FROM(tbl_instrumenttype INNER JOIN tbl_instrument ON tbl_instrumenttype.InstrumentType = tbl_instrument.InstrumentType) ";
            query += "WHERE tbl_instrumenttype.InstrumentType = @TYPE";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        it.instrumentType = reader.GetString(0);
                        it.pricePerHour = Database.getInstrumentTypePricePerHour(it.instrumentType, DateTime.Now.ToString("yyyy-MM-dd"));
                        it.totalQuantity = reader.GetInt32(2);

                        it.rentedQuantity = 0;
                        it.scheduleID = -1;
                        it.totalPrice = 0;
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return it;
        }

        //public static float getAdditionalFees(int scheduleID)
        //{
        //    float amount = 0;

        //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
        //    string query = "SELECT SUM(tbl_instrumentrented.Quantity * tbl_instrumenttype.PricePerHour) FROM ";
        //    query += "tbl_instrumentrented INNER JOIN tbl_instrumenttype ON tbl_instrumentrented.FK_InstrumentType_InstrumentType = tbl_instrumenttype.InstrumentType ";
        //    query += "WHERE tbl_instrumentrented.FK_Schedule_ScheduleID = @ID";

        //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    dbCommand.Parameters.AddWithValue("@ID", scheduleID);
        //    dbCommand.CommandTimeout = 60;
            

        //    Console.WriteLine(query);

        //    try
        //    {
        //        //open and close connection
        //        dbConnection.Open();
                
        //        string tmpPrice = dbCommand.ExecuteScalar().ToString();
        //        if (tmpPrice != "")
        //        {
        //            amount = float.Parse(tmpPrice);
        //        }
                
                
        //        dbConnection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Database Error: " + ex.Message);
        //    }

        //    return amount;
        //}

        public static void updateSchedule(Schedule schedule)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "UPDATE `tbl_schedule` ";
            query += "SET `FK_Band_BandID`= @BID,`FK_ScheduleType_ScheduleTypeID`= @STID,`FK_Representative_RepresentativeName`= @RNAME, ";
            query += "`ScheduleDate`= @DATE,`StartTime`= @TIME,`DurationInHours`= @DURATION,`IsPaid`= @ISPAID ";
            query += "WHERE `ScheduleID`= @ID ";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@BID", schedule.bandID);
            dbCommand.Parameters.AddWithValue("@STID", schedule.scheduleTypeID);
            dbCommand.Parameters.AddWithValue("@RNAME", schedule.representativeName);
            dbCommand.Parameters.AddWithValue("@DATE", schedule.scheduleDate);
            dbCommand.Parameters.AddWithValue("@TIME", schedule.startTime);
            dbCommand.Parameters.AddWithValue("@DURATION", schedule.duration);
            dbCommand.Parameters.AddWithValue("@ISPAID", schedule.isPaid);
            dbCommand.Parameters.AddWithValue("@ID", schedule.scheduleID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                //notify of successful operation
                MessageBox.Show($"Successfully edited Schedule #{schedule.scheduleID}!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void updateInstrumentsRented(int scheduleID, List<InstrumentType> instrumentsRented)
        {
            foreach (InstrumentType it in instrumentsRented)
            {
                MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

                string query = "UPDATE `tbl_instrumentrented` SET `Quantity`=@QUANTITY WHERE `FK_Schedule_ScheduleID`=@ID AND `FK_InstrumentType_InstrumentType`=@TYPE";
                
                MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@QUANTITY", it.rentedQuantity);
                dbCommand.Parameters.AddWithValue("@ID", scheduleID);
                dbCommand.Parameters.AddWithValue("@TYPE", it.instrumentType);
                dbCommand.CommandTimeout = 60;

                MySqlDataReader reader;

                Console.WriteLine(query);

                try
                {
                    //open and close connection
                    dbConnection.Open();
                    reader = dbCommand.ExecuteReader();
                    
                    dbConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
        }

        public static float getScheduleTypePrice(int scheduleTypeID, string date)
        {
            float price = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "SELECT PricePerHour FROM tbl_scheduletypeprice ";
            query += "WHERE `ScheduleTypeID`= @STID AND `DateEffective`<= @DATE ";
            query += "ORDER BY `DateEffective` DESC LIMIT 1";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@STID", scheduleTypeID);
            dbCommand.Parameters.AddWithValue("@DATE", DateTime.Parse(date).ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        price = reader.GetFloat(0);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return price;
        }

        public static bool isBandExisting(string bandName)
        {
            bool bandFound = false;
            
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `BandID` FROM tbl_band WHERE BandName = @BNAME";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@BNAME", bandName);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    bandFound = true;
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return bandFound;
        }

        public static void addBand(string bandName, bool hasPenalty)
        {
            //add band only if its not existing yet
            if (!isBandExisting(bandName))
            {
                MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

                string query = "INSERT INTO `tbl_band`(`BandName`, `HasPenalty`) VALUES (@BNAME, @PENALTY)";

                MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@BNAME", bandName);
                dbCommand.Parameters.AddWithValue("@PENALTY", hasPenalty);
                dbCommand.CommandTimeout = 60;

                MySqlDataReader reader;

                Console.WriteLine(query);

                try
                {
                    //open and close connection
                    dbConnection.Open();
                    reader = dbCommand.ExecuteReader();

                    Console.WriteLine("New Band added!");

                    dbConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            } else
            {
                Console.WriteLine("Band already exists, update haspenalty instead.");

                updateBandPenalty(getBandID(bandName), hasPenalty);
            }
        }

        public static bool isRepresentativeExisting(string repName)
        {
            bool repFound = false;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT * FROM tbl_representative WHERE RepresentativeName=@RNAME";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@RNAME", repName);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    repFound = true;
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return repFound;
        }

        public static void addRepresentative(string repName, string contactNumber)
        {
            //add band only if its not existing yet
            if (!isRepresentativeExisting(repName))
            {
                MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

                string query = "INSERT INTO `tbl_representative`(`RepresentativeName`, `ContactNumber`) VALUES (@RNAME, @CONTACT)";

                MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@RNAME", repName);
                dbCommand.Parameters.AddWithValue("@CONTACT", contactNumber);
                dbCommand.CommandTimeout = 60;

                MySqlDataReader reader;

                Console.WriteLine(query);

                try
                {
                    //open and close connection
                    dbConnection.Open();
                    reader = dbCommand.ExecuteReader();

                    Console.WriteLine("New Representative added!");

                    dbConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Representative already exists");
            }
        }

        public static void addSchedule(Schedule newSchedule, List<InstrumentType> instrumentsRented)
        {
            int scheduleID = -1;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "INSERT INTO `tbl_schedule`(`FK_Band_BandID`, `FK_ScheduleType_ScheduleTypeID`, `FK_Representative_RepresentativeName`, `DateAdded`, `ScheduleDate`, `StartTime`, `DurationInHours`, `IsPaid`) ";
            query += "VALUES(@BANDID, @STYPEID, @RNAME, @DATEADDED, @SCHEDDATE, @STARTTIME, @DURATION, @ISPAID);";
            query += "SELECT LAST_INSERT_ID();";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@BANDID", newSchedule.bandID);
            dbCommand.Parameters.AddWithValue("@STYPEID", newSchedule.scheduleTypeID);
            dbCommand.Parameters.AddWithValue("@RNAME", newSchedule.representativeName);
            dbCommand.Parameters.AddWithValue("@DATEADDED", DateTime.Now.ToString("yyyy-MM-dd"));
            dbCommand.Parameters.AddWithValue("@SCHEDDATE", newSchedule.scheduleDate);
            dbCommand.Parameters.AddWithValue("@STARTTIME", DateTime.Parse(newSchedule.startTime).ToString("HH:mm"));
            dbCommand.Parameters.AddWithValue("@DURATION", newSchedule.duration);
            dbCommand.Parameters.AddWithValue("@ISPAID", newSchedule.isPaid);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        scheduleID = reader.GetInt32(0);
                    }
                }

                DateTime date = DateTime.Parse(newSchedule.scheduleDate);
                DateTime time = DateTime.Parse(newSchedule.startTime);

                string message = "Successfully booked new schedule for:\n";
                message += $"{newSchedule.representativeName} on {date.ToString("dddd, dd MMMM yyyy")} at {time.ToString("hh:mm tt")}.";
                MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            rentInstruments(scheduleID, instrumentsRented);
        }

        public static void rentInstruments(int scheduleID, List<InstrumentType> instrumentsRented)
        {
            foreach (InstrumentType it in instrumentsRented)
            {
                //add to database only if user entered amount
                if (it.rentedQuantity > 0)
                {
                    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

                    string query = "INSERT INTO `tbl_instrumentrented`(`FK_InstrumentType_InstrumentType`, `FK_Schedule_ScheduleID`, `Quantity`) ";
                    query += "VALUES(@TYPE, @SID, @QTY)";

                    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
                    dbCommand.Parameters.AddWithValue("@TYPE", it.instrumentType);
                    dbCommand.Parameters.AddWithValue("@SID", scheduleID);
                    dbCommand.Parameters.AddWithValue("@QTY", it.rentedQuantity);
                    dbCommand.CommandTimeout = 60;

                    MySqlDataReader reader;

                    Console.WriteLine(query);

                    try
                    {
                        //open and close connection
                        dbConnection.Open();
                        reader = dbCommand.ExecuteReader();

                        dbConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message);
                    }
                }
            }
        }

        public static void deleteSchedule(int scheduleID)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "UPDATE tbl_schedule SET `IsDeleted`=1 WHERE `ScheduleID`=@ID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                MessageBox.Show($"Successfully deleted Schedule #{scheduleID}.");

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static List<Schedule> getNextSchedules(DateTime date, int limit)
        {
            List<Schedule> listOfSchedules = new List<Schedule>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT tbl_band.BandName, `FK_Representative_RepresentativeName`, `ScheduleDate`, `StartTime`, `ScheduleID`";
            query += "FROM(tbl_schedule INNER JOIN tbl_band on tbl_schedule.FK_Band_BandID = tbl_band.BandID) ";
            query += "WHERE((CAST(CONCAT(`ScheduleDate`, ' ', `StartTime`) as datetime) >= @DATETIME) AND (IsDeleted = 0)) ";
            query += "ORDER BY `ScheduleDate` ASC, `StartTime` ASC ";
            query += "LIMIT @LIMIT";
            
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@DATETIME", date.ToString("yyyy-MM-dd HH:mm"));
            dbCommand.Parameters.AddWithValue("@LIMIT", limit);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listOfSchedules.Add(new Schedule
                        {
                            bandName = reader.GetString(0),
                            representativeName = reader.GetString(1),
                            scheduleDate = DateTime.Parse(reader.GetString(2)).ToString("yyyy-MM-dd"),
                            startTime = DateTime.Parse(reader.GetString(3)).ToString("hh:mm tt"),
                            scheduleID = reader.GetInt32(4)
                        });
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listOfSchedules;
        }

        public static void updateInstrumentsRented()
        {

        }
        
        //INSTRUMENT SQL
        public static void addInstrument(Instrument instrument)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_instrument`(`InstrumentID`, `InstrumentModel`, `InstrumentType`, `InstrumentDescription`) ";
            query += "VALUES (NULL,@MODEL,@TYPE,@DESCRIPTION) ";
            
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MODEL", instrument.instrumentModel);
            dbCommand.Parameters.AddWithValue("@TYPE", instrument.instrumentType);
            dbCommand.Parameters.AddWithValue("@DESCRIPTION", instrument.instrumentDescription);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;
             
            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader(); //execute sql insert into
                
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void editInstrument(Instrument instrument)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "UPDATE `tbl_instrument` SET `InstrumentModel`=@MODEL,`InstrumentType`=@TYPE,`InstrumentDescription`=@DESC ";
            query += "WHERE `InstrumentID`=@ID AND `IsDeleted`=0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MODEL", instrument.instrumentModel);
            dbCommand.Parameters.AddWithValue("@TYPE", instrument.instrumentType);
            dbCommand.Parameters.AddWithValue("@DESC", instrument.instrumentDescription);
            dbCommand.Parameters.AddWithValue("@ID", instrument.instrumentID);
            dbCommand.CommandTimeout = 60;

            Console.WriteLine(query);

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader(); //execute sql udpate
                
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void deleteInstrument(int instrumentID)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "UPDATE `tbl_instrument` SET `IsDeleted`=1 WHERE `InstrumentID`=@ID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", instrumentID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                Console.WriteLine(query);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static List<Instrument> getInstrumentsList(string searchKey, string type)
        {
            //temporary storage list to return
            List<Instrument> listOfInstruments = new List<Instrument>();

            //SQL
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            //query
            string query = "";
            query += "SELECT * FROM `tbl_instrument` ";
            
            query += "WHERE "; //add WHERE only if there is a search word OR there is a filter type

            if (searchKey != "")
            {
                query += "(`InstrumentID` LIKE '%" + searchKey + "%' OR ";
                query += "`InstrumentModel` LIKE '%" + searchKey + "%' OR ";
                query += "`InstrumentType` LIKE '%" + searchKey + "%' OR ";
                query += "`InstrumentDescription` LIKE '%" + searchKey + "%') ";
            }

            if (searchKey != "" && type != "All")
            {
                query += "AND (`InstrumentType`=@TYPE)";  //add only AND keyword if search key is not empty 
            } else if (searchKey == "" && type != "All")
            {
                query += "(`InstrumentType`=@TYPE)";      //else dont add AND
            }

            if (searchKey != "" || type != "All")
            {
                query += " AND (`IsDeleted`=0)";
            } else
            {
                query += " (`IsDeleted`=0)";
            }
            
            
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            Console.WriteLine(query);

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listOfInstruments.Add(new Instrument(reader.GetInt32(0),
                                                             reader.GetString(1),
                                                             reader.GetString(2),
                                                             reader.GetString(3))
                                                             );
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listOfInstruments;
        }

        public static List<InstrumentType> getAllInstrumentTypes()
        {
            //temporary storage of return type list of available types
            List<InstrumentType> instrumentTypes = new List<InstrumentType>();

            //sql connection
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            //query
            string query = "SELECT * FROM `tbl_instrumenttype`";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        instrumentTypes.Add(new InstrumentType
                        {
                            instrumentType = reader.GetString(0),
                            pricePerHour = Database.getInstrumentTypePricePerHour(reader.GetString(0), DateTime.Now.ToString("yyyy-MM-dd")),
                            totalQuantity = getTotalAvailableInstrument(reader.GetString(0)),

                        }); //store database available types in arraylist
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return instrumentTypes; //return all available data to method caller
        } //all instrument types
        
        public static List<InstrumentType> getScheduleInstrumentTypes(int scheduleID, string date)
        {
            //temporary storage of return type list of available types
            List<InstrumentType> instrumentTypes = new List<InstrumentType>();

            //sql connection
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            //query
            string query = "SELECT * FROM `tbl_instrumenttype`";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //to lessen sql queries, use this method
                        InstrumentType tmpIT = new InstrumentType();
                        tmpIT.instrumentType = reader.GetString(0);
                        tmpIT.pricePerHour = Database.getInstrumentTypePricePerHour(reader.GetString(0), date);
                        tmpIT.totalQuantity = getTotalAvailableInstrument(reader.GetString(0));
                        tmpIT.rentedQuantity = Database.getCountInstrumentRentedBySchedule(scheduleID, reader.GetString(0));
                        tmpIT.totalPrice = tmpIT.rentedQuantity * tmpIT.pricePerHour;

                        instrumentTypes.Add(tmpIT); 
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return instrumentTypes;
        }

        public static List<InstrumentType> removeZeroQuantityInstruments(List<InstrumentType> instrumentTypes)
        {
            List<InstrumentType> listToReturn = new List<InstrumentType>();

            foreach (InstrumentType it in instrumentTypes)
            {
                if (it.totalQuantity > 0)
                {
                    listToReturn.Add(it);
                }
            }

            return listToReturn;
        }

        public static int getCountInstrumentRentedBySchedule(int scheduleID, string type)
        {
            //temporary storage of return type list of available types
            int countType = 0;

            //sql connection
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "SELECT `Quantity` FROM `tbl_instrumentrented` ";
            query += "WHERE `FK_Schedule_ScheduleID`= @SID AND `FK_InstrumentType_InstrumentType`= @TYPE";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@SID", scheduleID);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        countType = reader.GetInt32(0);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return countType;
        }

        public static Instrument getInstrument(int instrumentID)
        {
            Instrument instrument;
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT * FROM `tbl_instrument` WHERE `InstrumentID`=@ID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", instrumentID);
            dbCommand.CommandTimeout = 60;

            Console.WriteLine(query);

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        instrument = new Instrument(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        dbConnection.Close();
                        return instrument;
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            //return null if instrument not found, handle null exception on method caller
            return null;
        }

        public static bool isInstrumentTypeExisting(string type)
        {
            //store bool if instrument type already found
            bool typeFound = false; //false - default, change to true if similar found

            //query check if type already exists
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT * FROM `tbl_instrumenttype` WHERE `InstrumentType`=@TYPE";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    typeFound = true;
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return typeFound;
        }
        
        public static void addInstrumentType(InstrumentType it)
        {
            //if type does not exist yet
            if (!isInstrumentTypeExisting(it.instrumentType))
            {
                //query check if type already exists
                MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

                string query = "INSERT INTO `tbl_instrumenttype` (`InstrumentType`) VALUES (@TYPE);";

                MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
                dbCommand.Parameters.AddWithValue("@TYPE", it.instrumentType.ToUpper());
                dbCommand.CommandTimeout = 60;

                MySqlDataReader reader;

                try
                {
                    //open and close connection
                    dbConnection.Open();
                    reader = dbCommand.ExecuteReader(); //execute insert into command

                    dbConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message);
                }

                addInstrumentTypePricePerHour(it.instrumentType, it.pricePerHour);
            }
        }

        public static void addInstrumentTypePricePerHour(string type, float price)
        {
            //add its priceperhour in table
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_instrumenttypeprice`(`InstrumentType`, `PricePerHour`, `DateEffective`) ";
            query += "VALUES(@TYPE, @PRICE, @DATE)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@PRICE", price);
            dbCommand.Parameters.AddWithValue("@DATE", (DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00")); //add date effective default today at 12am
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader(); //execute insert into command

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void updateInstrumentTypePrice(string type, float newPrice, string date)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_instrumenttypeprice`(`InstrumentType`, `PricePerHour`, `DateEffective`) VALUES (@TYPE, @PRICE, @DATE)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@PRICE", newPrice);
            dbCommand.Parameters.AddWithValue("@DATE", date);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        
        public static int countInstruments()
        {
            int instrumentCount = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT COUNT(*) FROM tbl_instrument WHERE IsDeleted = 0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();

                instrumentCount = Convert.ToInt32(dbCommand.ExecuteScalar());
                
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return instrumentCount;
        }

        public static List<string> getUnusedInstrumentTypes(int scheduleID)
        {
            List<string> listUnusedTypes = new List<string>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT tbl_instrumenttype.InstrumentType FROM ";
            query += "(tbl_instrumenttype LEFT JOIN tbl_instrumentrented ON tbl_instrumenttype.InstrumentType = tbl_instrumentrented.FK_InstrumentType_InstrumentType AND tbl_instrumentrented.FK_Schedule_ScheduleID = @ID) ";
            query += "WHERE tbl_instrumentrented.FK_InstrumentType_InstrumentType IS NULL ";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listUnusedTypes.Add(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listUnusedTypes;
        }

        public static void rentInstrumentType(int scheduleID, string type, int quantity)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "INSERT INTO `tbl_instrumentrented`(`FK_InstrumentType_InstrumentType`, `FK_Schedule_ScheduleID`, `Quantity`) ";
            query += "VALUES (@TYPE,@ID,@QUANTITY)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
            dbCommand.Parameters.AddWithValue("@QUANTITY", quantity);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                MessageBox.Show($"Successfully rented {type} for Schedule #{scheduleID}", "Rent Instrument");

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            
        }
        
        public static void deleteInstrumentsRented(int scheduleID)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "DELETE FROM `tbl_instrumentrented` WHERE `FK_Schedule_ScheduleID`=@ID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@ID", scheduleID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                Console.WriteLine("Removed existing instrument rentals.");

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static float getInstrumentTypePricePerHour(string type, string date)
        {
            float price = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "SELECT PricePerHour FROM `tbl_instrumenttypeprice` ";
            query += "WHERE `InstrumentType`= @TYPE AND `DateEffective`<= @DATE ";
            query += "ORDER BY `DateEffective` DESC LIMIT 1";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@TYPE", type);
            dbCommand.Parameters.AddWithValue("@DATE", DateTime.Parse(date).ToString("yyyy-MM-dd"));
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        price = reader.GetFloat(0);
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return price;
        }

        //ACCOUNTS SQL
        public static void addAccount(User user)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "INSERT INTO `tbl_user`(`Username`, `PasswordHash`, `IsAdmin`) ";
            query += "VALUES (@USERNAME, SHA1(@PASSWORD), @ISADMIN)";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@USERNAME", user.username);
            dbCommand.Parameters.AddWithValue("@PASSWORD", user.password);
            dbCommand.Parameters.AddWithValue("@ISADMIN", user.isAdmin);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                MessageBox.Show($"Account '{user.username}' successfully added!");

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void updateAccount(User user)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "UPDATE `tbl_user` SET `PasswordHash`=SHA1(@PASSWORD) WHERE UserID = @USERID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@USERID", user.userID);
            dbCommand.Parameters.AddWithValue("@PASSWORD", user.password);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                MessageBox.Show($"Successfully changed password of {user.username}.",
                    "Update Account",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static void deleteAccount(User user)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "UPDATE `tbl_user` SET `IsDeleted`=1 WHERE UserID = @USERID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@USERID", user.userID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                MessageBox.Show($"Successfully deleted account of {user.username}.",
                    "Delete Account",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static bool isUsernameUsed(string username)
        {
            bool isUsernameUsed = false;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "";
            query += "SELECT Username FROM tbl_user WHERE Username=@username AND IsDeleted=0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@username", username);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                //check username if used
                if (reader.HasRows)
                {
                    isUsernameUsed = true;
                }
                else
                {
                    isUsernameUsed = false;
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return isUsernameUsed;
        }

        
        //BANDS/REPRESENTATIVE SQL
        public static List<string> getBands()
        {
            List<string> listBands = new List<string>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT BandName FROM `tbl_band`";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listBands.Add(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listBands;
        }

        public static List<string> getRepresentatives()
        {
            List<string> listRepresentatives = new List<string>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT RepresentativeName FROM tbl_representative";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listRepresentatives.Add(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listRepresentatives;
        }

        public static string getContactNumber(string representativeName)
        {
            string contactNumber = "";

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT ContactNumber FROM tbl_representative WHERE RepresentativeName=@NAME";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@NAME", representativeName);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        contactNumber = reader.GetString(0);
                    }
                }
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return contactNumber;
        }

        public static int getBandID(string bandName)
        {
            int bandID = -1;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `BandID` FROM tbl_band WHERE `BandName`=@NAME";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@NAME", bandName);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        bandID = reader.GetInt32(0);
                    }
                }
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return bandID;
        }

        public static bool getBandPenalty(int bandID)
        {
            bool hasPenalty = false;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT `HasPenalty` FROM `tbl_band` WHERE `BandID`=@BID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@BID", bandID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        hasPenalty = reader.GetBoolean(0);
                    }
                }
                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return hasPenalty;
        }

        public static void updateBandPenalty(int bandID, bool hasPenalty)
        {
            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "UPDATE `tbl_band` SET `HasPenalty`=@PENALTY WHERE `BandID`=@BID";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@PENALTY", hasPenalty);
            dbCommand.Parameters.AddWithValue("@BID", bandID);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                Console.WriteLine("Edited Band Penalty");

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
        }

        public static List<string> getBandListRented(DateTime date)
        {
            List<string> listOfBands = new List<string>();

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);
            
            string query = "SELECT DISTINCT tbl_band.BandName ";
            query += "FROM(tbl_schedule INNER JOIN tbl_band ON tbl_schedule.FK_Band_BandID = tbl_band.BandID) ";
            query += "WHERE MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR AND IsDeleted = 0 ";
            query += "ORDER BY BandName ASC";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
            dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listOfBands.Add(reader.GetString(0));
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return listOfBands;
        }


        //DASHBOARD SQL
        //public static float getScheduleTotalMonthlyRevenue(DateTime date)
        //{
        //    float price = 0;

        //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

        //    string query = "SELECT SUM(tbl_scheduletype.PricePerHour * tbl_schedule.DurationInHours) as 'TotalPrice' ";
        //    query += "FROM(tbl_schedule INNER JOIN tbl_scheduletype ON tbl_schedule.FK_ScheduleType_ScheduleTypeID = tbl_scheduletype.ScheduleTypeID) ";
        //    query += "WHERE (MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR) AND tbl_schedule.IsDeleted = 0";

        //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
        //    dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
        //    dbCommand.CommandTimeout = 60;

        //    MySqlDataReader reader;

        //    Console.WriteLine(query);

        //    try
        //    {
        //        open and close connection
        //        dbConnection.Open();
        //        reader = dbCommand.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            if (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    price = reader.GetFloat(0);
        //                }
        //            }
        //        }

        //        dbConnection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Database Error: " + ex.Message);
        //    }

        //    return price;
        //}

        //public static float getInstrumentTotalMonthlyRevenue(DateTime date)
        //{
        //    float price = 0;

        //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

        //    string query = "SELECT SUM(tbl_instrumenttype.PricePerHour * tbl_schedule.DurationInHours) ";
        //    query += "FROM((tbl_schedule INNER JOIN tbl_instrumentrented ON tbl_schedule.ScheduleID = tbl_instrumentrented.FK_Schedule_ScheduleID) ";
        //    query += "INNER JOIN tbl_instrumenttype ON tbl_instrumentrented.FK_InstrumentType_InstrumentType = tbl_instrumenttype.InstrumentType) ";
        //    query += "WHERE (MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR) AND tbl_schedule.IsDeleted = 0";

        //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
        //    dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
        //    dbCommand.CommandTimeout = 60;

        //    MySqlDataReader reader;

        //    Console.WriteLine(query);

        //    try
        //    {
        //        //open and close connection
        //        dbConnection.Open();
        //        reader = dbCommand.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            if (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    price = reader.GetFloat(0);
        //                }
        //            }
        //        }

        //        dbConnection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Database Error: " + ex.Message);
        //    }

        //    return price;
        //}

        //public static float getScheduleTotalMonthlyRevenue(DateTime date)
        //{
        //    float price = 0;

        //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

        //    string query = "SELECT SUM(tbl_scheduletypeprice.PricePerHour * tbl_schedule.DurationInHours) as 'TotalPrice' ";
        //    query += "FROM(tbl_schedule INNER JOIN tbl_scheduletypeprice ON tbl_schedule.FK_ScheduleType_ScheduleTypeID = tbl_scheduletypeprice.ScheduleTypeID) ";
        //    query += "WHERE (MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR) AND tbl_schedule.IsDeleted = 0 AND tbl_scheduletypeprice.DateEffective <= tbl_schedule.DateAdded";
        //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
        //    dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
        //    dbCommand.CommandTimeout = 60;

        //    MySqlDataReader reader;

        //    Console.WriteLine(query);

        //    try
        //    {
        //        //open and close connection
        //        dbConnection.Open();
        //        reader = dbCommand.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            if (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    price = reader.GetFloat(0);
        //                }
        //            }
        //        }

        //        dbConnection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Database Error: " + ex.Message);
        //    }

        //    return price;
        //    //    float price = 0;

        //    //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

        //    //    string query = "SELECT SUM(tbl_scheduletype.PricePerHour * tbl_schedule.DurationInHours) as 'TotalPrice' ";
        //    //    query += "FROM(tbl_schedule INNER JOIN tbl_scheduletype ON tbl_schedule.FK_ScheduleType_ScheduleTypeID = tbl_scheduletype.ScheduleTypeID) ";
        //    //    query += "WHERE (MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR) AND tbl_schedule.IsDeleted = 0";

        //    //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    //    dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
        //    //    dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
        //    //    dbCommand.CommandTimeout = 60;

        //    //    MySqlDataReader reader;

        //    //    Console.WriteLine(query);

        //    //    try
        //    //    {
        //    //        open and close connection
        //    //        dbConnection.Open();
        //    //        reader = dbCommand.ExecuteReader();

        //    //        if (reader.HasRows)
        //    //        {
        //    //            if (reader.Read())
        //    //            {
        //    //                if (!reader.IsDBNull(0))
        //    //                {
        //    //                    price = reader.GetFloat(0);
        //    //                }
        //    //            }
        //    //        }

        //    //        dbConnection.Close();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        MessageBox.Show("Database Error: " + ex.Message);
        //    //    }

        //    //    return price;
        //}

        //public static float getInstrumentTotalMonthlyRevenue(DateTime date)
        //{
        //    float price = 0;

        //    MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

        //    string query = "SELECT SUM(tbl_instrumenttypeprice.PricePerHour * tbl_schedule.DurationInHours) ";
        //    query += "FROM((tbl_schedule INNER JOIN tbl_instrumentrented ON tbl_schedule.ScheduleID = tbl_instrumentrented.FK_Schedule_ScheduleID) ";
        //    query += "INNER JOIN tbl_instrumenttypeprice ON tbl_instrumentrented.FK_InstrumentType_InstrumentType = tbl_instrumenttypeprice.InstrumentType) ";
        //    query += "WHERE (MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR) AND tbl_schedule.IsDeleted = 0 AND tbl_instrumenttypeprice.DateEffective <= tbl_schedule.DateAdded";

        //    MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
        //    dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
        //    dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
        //    dbCommand.CommandTimeout = 60;

        //    MySqlDataReader reader;

        //    Console.WriteLine(query);

        //    try
        //    {
        //        //open and close connection
        //        dbConnection.Open();
        //        reader = dbCommand.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            if (reader.Read())
        //            {
        //                if (!reader.IsDBNull(0))
        //                {
        //                    price = reader.GetFloat(0);
        //                }
        //            }
        //        }

        //        dbConnection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Database Error: " + ex.Message);
        //    }

        //    return price;
        //}

        public static int countBandsRented(DateTime date)
        {
            int numberOfBands = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT COUNT(tbl_schedule.FK_Band_BandID) FROM tbl_schedule WHERE MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR AND IsDeleted=0";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
            dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
            dbCommand.CommandTimeout = 60;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();

                //fetch count - returns 0 if nothing fetched
                numberOfBands = Convert.ToInt32(dbCommand.ExecuteScalar());

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return numberOfBands;
        }

        public static int countInstrumentsRented(DateTime date)
        {
            int countInstruments = 0;

            MySqlConnection dbConnection = new MySqlConnection(SystemCore.connectionString);

            string query = "SELECT SUM(tbl_instrumentrented.Quantity) ";
            query += "FROM(tbl_instrumentrented INNER JOIN tbl_schedule ON tbl_instrumentrented.FK_Schedule_ScheduleID = tbl_schedule.ScheduleID) ";
            query += "WHERE MONTH(ScheduleDate) = @MONTH AND YEAR(ScheduleDate) = @YEAR AND IsDeleted = 0 ";

            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.Parameters.AddWithValue("@MONTH", date.Month);
            dbCommand.Parameters.AddWithValue("@YEAR", date.Year);
            dbCommand.CommandTimeout = 60;

            MySqlDataReader reader;

            Console.WriteLine(query);

            try
            {
                //open and close connection
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            countInstruments = reader.GetInt32(0);
                        }
                    }
                }

                dbConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return countInstruments;
        }
    }
}
