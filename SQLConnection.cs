using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace SqlCon
{
    public class SQLConnection
    {
        readonly SqlConnection SqlCon = new SqlConnection();

        /// <summary>
        /// Constractour Initialisation the instance from the class.
        /// </summary>
        /// <param name="Connectionstring">Zain to be processed.</param>
        /// <returns>SQLConnection</returns>
        // Constractour
        public SQLConnection(string Connectionstring) => Get_SQl_Con(Connectionstring);


        // read connection string from string will come from Cls_Read_Con
        private void Get_SQl_Con(string Con_string) => SqlCon.ConnectionString = Con_string;



        // return SqlConnection maybe i need it 
        public SqlConnection Get_SQL_Con() => (Is_Connected()) ? SqlCon : null;

        // return ConnectionString maybe i need it 
        public string Get_ConStr() => (Is_Connected()) ? SqlCon.ConnectionString.ToString() : null;


        // if connection close open it
        public void Open_Con() { if (SqlCon.State == ConnectionState.Closed) SqlCon.Open(); }


        //if connection open close it
        public void Close_Con()
        { if (SqlCon.State == ConnectionState.Open) SqlCon.Close(); }

        public void Force_Close()
        {
            string TempCon = Get_ConStr();
            SqlCon.Dispose();
            SqlCon.ConnectionString = TempCon;
        }


        public bool Refreash_Con()
        {
            try
            {
                Open_Con();
                Close_Con();
                return true;
            }
            catch (Exception)
            {
                try
                {
                    Open_Con();
                    Close_Con();
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }

            }

        }
        // boolean for check if connection staple , Thrown new exmircc
        private bool Is_Connected(bool Throw = false)
        {
            try
            {
                Open_Con();
                Close_Con();
                return true;
            }
            catch
            {
                if (Throw)
                    throw;
                return false;
            }


        }

        // Get Auto Number from Table 
        public int Get_AutoNum(string Col_Name, string Tbl_Name)

        {
            if (Is_Connected())
            {
                using (SqlCommand cmd = new SqlCommand
                    ("select ISNULL( max( " + Col_Name + "),0)+1 from " + Tbl_Name + " ", SqlCon))
                {
                    Open_Con();
                    int NO = (int)cmd.ExecuteScalar();
                    Close_Con();
                    return NO;
                }
            }
            return -1;
        }

        // Get Max ID without Auto +1
        public int Get_Last_Index(string Col_Name, string Tbl_Name)

        {
            if (Is_Connected())
            {
                using (SqlCommand cmd = new SqlCommand
                    ("select ISNULL( max( " + Col_Name + "),0) from " + Tbl_Name + " ", SqlCon))
                {
                    Open_Con();
                    int NO = (int)cmd.ExecuteScalar();
                    Close_Con();
                    return NO;

                }
            }
            return 0;
        }

        // fill DataTable with select Query using string 
        public DataTable Select_Query(string Select_Query)
        {
            DataTable dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && Select_Query.Length > 11)
                    using (SqlDataAdapter da = new SqlDataAdapter(Select_Query, SqlCon))
                        da.Fill(dt);
            }
            catch /*(Exception ex)*/ { throw; }
            return dt;
        }

        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(string Select_Query)
        {
            DataTable dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && Select_Query.Length > 1)
                    using (SqlDataAdapter da = new SqlDataAdapter(Select_Query, SqlCon))
                        da.Fill(dt);
            }
            catch { throw; }

            return (dt.Rows.Count > 0);
        }


        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(string Select_Query, ref DataTable dt)
        {
            dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && Select_Query.Length > 1)
                    using (SqlDataAdapter da = new SqlDataAdapter(Select_Query, SqlCon))
                        da.Fill(dt);
            }
            catch { throw; }

            return (dt.Rows.Count > 0);
        }

        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(SqlCommand CMD_Select_Query)
        {
            DataTable dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && CMD_Select_Query != null)
                {
                    CMD_Select_Query.Connection = SqlCon;
                    using (SqlDataAdapter da = new SqlDataAdapter(CMD_Select_Query))
                        da.Fill(dt);
                }
            }
            catch { throw; }

            return (dt.Rows.Count > 0);
        }


        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(SqlCommand CMD_Select_Query, ref DataTable dt)
        {
            dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && CMD_Select_Query != null)
                {
                    CMD_Select_Query.Connection = SqlCon;
                    using (SqlDataAdapter da = new SqlDataAdapter(CMD_Select_Query))
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count > 0) return true;
                    };
                }
            }
            catch
            {
                throw;
            }
            return false;
        }


        // fill DataTable with select Query using CMD 
        public DataTable Select_Query(SqlCommand SqlCmd)
        {

            DataTable dt = new DataTable(); dt.Rows.Clear();
            if (Is_Connected())
            {
                try
                {
                    SqlCmd.Connection = SqlCon;

                    using (SqlDataAdapter da = new SqlDataAdapter(SqlCmd))
                    {
                        Open_Con();
                        dt.Rows.Clear();
                        da.Fill(dt);
                        Close_Con();
                        SqlCmd.Dispose();
                        return dt;
                    }
                }
                catch
                {
                    throw;
                }

            }

            return dt;
        }


        public SqlDataAdapter Select_DataAdapter(string Cmd)
        {


            if (Is_Connected())
            {
                try
                {
                    SqlDataAdapter Da = new SqlDataAdapter(Cmd, SqlCon);
                    return Da;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return null;
        }

        public SqlDataAdapter Select_DataAdapter(SqlCommand SqlCmd)
        {

            if (Is_Connected())
            {
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlDataAdapter Da = new SqlDataAdapter(SqlCmd);
                    return Da;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return null;
        }

        public bool Fill_Dataset(string Cmd, ref DataSet dataSet, bool throws = false)
        {


            if (Is_Connected())
                try
                {
                    dataSet.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(Cmd, SqlCon);
                    Da.Fill(dataSet);
                    return true;
                }
                catch (Exception)
                {
                    if (throws)
                        throw;
                    else
                        return false;
                }

            return false;


        }



        public DataSet Fill_Return_Dataset(SqlCommand SqlCmd, DataSet dataSet)
        {

            if (Is_Connected())
                try
                {
                    SqlCmd.Connection = SqlCon;
                    dataSet.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(SqlCmd);
                    Da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception)
                {
                    throw;
                }

            return null;
        }

        public DataSet Fill_Return_Dataset(string Cmd, DataSet dataSet)
        {


            if (Is_Connected())
                try
                {
                    dataSet.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(Cmd, SqlCon);
                    Da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception)
                {
                    throw;
                }

            return null;


        }



        public bool Fill_Datatable(SqlCommand SqlCmd, ref DataTable DT, bool throws = false)
        {
            if (Is_Connected())
                try
                {
                    SqlCmd.Connection = SqlCon;
                    DT.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(SqlCmd);
                    Da.Fill(DT);
                    return true;
                }
                catch (Exception)
                {
                    if (throws)
                        throw;
                    else
                        return false;
                }
            return false;
        }


        // Select Query Using Cmd Not string 
        public DataTable Fill_Return_DataTable(SqlCommand SqlCmd, DataTable DT)
        {

            if (Is_Connected())
                try
                {
                    SqlCmd.Connection = SqlCon;
                    DT.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(SqlCmd);
                    Da.Fill(DT);
                    return DT;
                }
                catch { throw; }

            return null;
        }

        public DataTable Fill_Return_DataTable(string Cmd, DataTable DT)
        {
            if (Is_Connected())
                try
                {
                    DT.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(Cmd, SqlCon);
                    Da.Fill(DT);
                    return DT;
                }
                catch { throw; }
            return null;
        }



        public bool Fill_Return_DataTable(SqlCommand SqlCmd, ref DataSet dataSet, bool throws = false)
        {

            if (Is_Connected())
                try
                {
                    SqlCmd.Connection = SqlCon;
                    dataSet.Clear();
                    SqlDataAdapter Da = new SqlDataAdapter(SqlCmd);
                    Da.Fill(dataSet);
                    return true;
                }
                catch (Exception)
                {
                    if (throws)
                        throw;
                    else
                        return false;
                }

            return false;
        }




        public SqlCommand Select_Cmd(string CMdString)
        {
            SqlCommand SqlCmd = new SqlCommand();

            if (Is_Connected())
            {
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlCmd.CommandText = CMdString;
                    Open_Con();
                    return SqlCmd;
                }
                catch
                {
                    throw;
                }

            }

            return SqlCmd;

        }



        public SqlDataReader Select_DataReader(string Select_Query)
        {
            DataTable dt = new DataTable(); dt.Rows.Clear();
            SqlCommand SqlCmd = new SqlCommand();
            SqlDataReader dr = null;
            if (Is_Connected())
            {
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlCmd.CommandText = Select_Query;
                    using (SqlDataAdapter da = new SqlDataAdapter(SqlCmd))
                    {
                        Open_Con();
                        dt.Rows.Clear();
                        da.Fill(dt);
                        dr = SqlCmd.ExecuteReader();
                        return dr;
                    }
                }
                catch
                {
                    throw;
                }
            }

            return dr;

        }

        public SqlDataReader Select_DataReader(SqlCommand SqlCmd)
        {

            DataTable dt = new DataTable(); dt.Rows.Clear();
            SqlDataReader dr = null;
            if (Is_Connected())
            {
                try
                {
                    SqlCmd.Connection = SqlCon;
                    using (SqlDataAdapter da = new SqlDataAdapter(SqlCmd))
                    {
                        Open_Con();
                        dt.Rows.Clear();
                        da.Fill(dt);
                        dr = SqlCmd.ExecuteReader();
                        return dr;
                    }
                }
                catch
                {
                    throw;
                }
            }

            return dr;
        }




        // insert Bulk Without check IDentity
        public bool Insert_Bulk(string TableName, DataTable dt, bool throws = false)
        {
            if (Is_Connected())
            {
                Open_Con();

                using (SqlTransaction Sqltransaction = SqlCon.BeginTransaction())
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlCon, SqlBulkCopyOptions.KeepIdentity, Sqltransaction))
                    {
                        bulkCopy.DestinationTableName = TableName;
                        try
                        {
                            bulkCopy.WriteToServer(dt);
                            Sqltransaction.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            Sqltransaction.Rollback();
                            if (throws)
                                throw;
                            return false;
                        }
                        finally
                        {
                            bulkCopy.Close();
                            Close_Con();
                        }
                    }
                }
            }
            return false;
        }

        // insert Bulk With check IDentity
        public bool Insert_Bulk_WithConst(string TableName, DataTable dt, bool throws = false)
        {
            if (Is_Connected())
            {
                Open_Con();

                using (SqlTransaction Sqltransaction = SqlCon.BeginTransaction())
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(SqlCon, SqlBulkCopyOptions.CheckConstraints, Sqltransaction))
                    {
                        bulkCopy.DestinationTableName = TableName;
                        try
                        {
                            bulkCopy.WriteToServer(dt);
                            Sqltransaction.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            Sqltransaction.Rollback();
                            if (throws)
                                throw;
                            return false;
                        }
                        finally
                        {
                            bulkCopy.Close();
                            Close_Con();
                        }
                    }
                }
            }




            return false;
        }

        // Run_Non_Query using string 
        public bool Run_Non_Query(string Cmd_String)
        {
            try
            {
                if (Is_Connected())
                    using (SqlCommand SqlCmd = new SqlCommand(Cmd_String, SqlCon))
                    {
                        Open_Con();
                        SqlCmd.ExecuteNonQuery();
                        Close_Con();
                        return true;
                    }
            }
            catch { throw; }

            return false;
        }

        // Run_Non_Query using CMD
        public bool Run_Non_Query(SqlCommand SqlCmd)
        {
            try
            {
                if (Is_Connected())
                {
                    SqlCmd.Connection = SqlCon;
                    using (SqlCmd)
                    {
                        Open_Con();
                        SqlCmd.ExecuteNonQuery();
                        Close_Con();
                        return true;
                    }
                }
            }
            catch { throw; }
            return false;
        }


        // Run_Transaction_Non_Query using string Cmd
        public bool Run_Transaction_Non_Query(string Cmd_String, bool throws = false)
        {
            if (Is_Connected())
            {
                Open_Con();
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();
                try
                {
                    using (SqlCommand SqlCmd = new SqlCommand(Cmd_String, SqlCon, Sqltransaction))
                    {
                        SqlCmd.ExecuteNonQuery();
                        Sqltransaction.Commit();
                        return true;
                    };
                }
                catch
                {
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                }

            }

            return false;

        }

        /// <summary>
        /// Run Transaction Non Query Get ID of the Inserted Row base on OUTPUT Inserted.Column Name before values or use SELECT SCOPE_IDENTITY().
        /// </summary>
        /// <param name="Cmd_String">Zain to be processed.</param>
        /// <returns>int Row ID</returns>
        // Run_Transaction_Non_Query_Get_ID and return the autoid value using string Cmd
        public int Run_Transaction_Non_Query_Get_ID(string Cmd_String, bool throws = false)
        {
            if (Is_Connected())
            {
                int id;
                Open_Con();
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();
                try
                {
                    using (SqlCommand SqlCmd = new SqlCommand(Cmd_String, SqlCon, Sqltransaction))
                    {
                        //SqlCmd.ExecuteNonQuery();
                        id = (int)SqlCmd.ExecuteScalar();
                        Sqltransaction.Commit();
                        return id;
                    };
                }
                catch
                {
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return 0;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                }
            }
            return 0;

        }

        public bool Run_Transaction_Non_Query(SqlCommand SqlCmd, bool throws = false)
        {
            if (Is_Connected())
            {
                Open_Con();
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlCmd.Transaction = Sqltransaction;
                    using (SqlCmd)
                    {
                        SqlCmd.ExecuteNonQuery();
                        Sqltransaction.Commit();
                        return true;
                    };
                }
                catch
                {
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    SqlCmd.CommandText = null;
                    SqlCmd.Parameters.Clear();
                    Close_Con();
                    GC.Collect();
                }
            }
            return false;
        }

        /// <summary>
        /// Run Transaction Non Query Get ID of the Inserted Row base on OUTPUT Inserted.Column Name before values or use SELECT SCOPE_IDENTITY().
        /// </summary>
        /// <param name="SqlCmd">Zain to be processed.</param>
        /// <returns>int Row ID</returns>
        // Run_Transaction_Non_Query_Get_ID and return the autoid value using string Cmd
        public int Run_Transaction_Non_Query_Get_ID(SqlCommand SqlCmd, bool throws = false)
        {
            if (Is_Connected())
            {
                int id;
                Open_Con();
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlCmd.Transaction = Sqltransaction;
                    using (SqlCmd)
                    {
                        id = (int)SqlCmd.ExecuteScalar();
                        Sqltransaction.Commit();
                        return id;
                    };
                }
                catch
                {
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return 0;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    SqlCmd.CommandText = null;
                    SqlCmd.Parameters.Clear();
                    Close_Con();
                    GC.Collect();
                }
            }
            return 0;
        }



        public bool Run_Transaction_Non_Query(ref SqlCommand SqlCmd, bool throws = false)
        {
            if (Is_Connected())
            {
                Open_Con();
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();
                try
                {
                    SqlCmd.Connection = SqlCon;
                    SqlCmd.Transaction = Sqltransaction;
                    using (SqlCmd)
                    {
                        SqlCmd.ExecuteNonQuery();
                        Sqltransaction.Commit();
                        return true;
                    };
                }
                catch
                {
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    SqlCmd.CommandText = null;
                    SqlCmd.Parameters.Clear();
                    Close_Con();
                    GC.Collect();
                }
            }
            return false;
        }

        public bool Run_Transaction_Non_Query(List<string> SqlCmds, bool throws = false)
        {
            // check connected
            if (Is_Connected())
            {
                // Open Connection First
                Open_Con();
                // Initial Transaction
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();

                try
                {
                    foreach (string Cmdstring in SqlCmds)
                    {
                        using (SqlCommand Cmd = new SqlCommand(Cmdstring, SqlCon, Sqltransaction))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    // Commit if end the Loop     
                    Sqltransaction.Commit();
                    // return true
                    return true;
                }
                catch/* (Exception ex)*/
                {
                    //   MessageBox.Show(ex.Message);
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                    SqlCmds.Clear();
                    GC.Collect();
                }
            }
            return false;

        }

        public bool Run_Transaction_Non_Query(ref List<string> SqlCmds, bool throws = false)
        {
            // check connected
            if (Is_Connected())
            {
                // Open Connection First
                Open_Con();
                // Initial Transaction
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();

                try
                {
                    foreach (string Cmdstring in SqlCmds)
                    {
                        using (SqlCommand Cmd = new SqlCommand(Cmdstring, SqlCon, Sqltransaction))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    // Commit if end the Loop     
                    Sqltransaction.Commit();
                    // return true
                    return true;
                }
                catch/* (Exception ex)*/
                {
                    //   MessageBox.Show(ex.Message);
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                    SqlCmds.Clear();
                    GC.Collect();
                }
            }
            return false;

        }

        // Run_Transaction_Non_Query using CMD  
        public bool Run_Transaction_Non_Query(List<SqlCommand> SqlCmd, bool throws = false)
        {
            // check connected
            if (Is_Connected())
            {
                // Open Connection First
                Open_Con();
                // Initial Transaction
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();

                try
                {
                    foreach (SqlCommand Cmd in SqlCmd)
                    {
                        Cmd.Transaction = Sqltransaction;
                        Cmd.Connection = SqlCon;
                        using (Cmd)
                            Cmd.ExecuteNonQuery();
                    }
                    // Commit if end the Loop     
                    Sqltransaction.Commit();
                    // return true
                    return true;

                }
                catch/* (Exception ex)*/
                {
                    //   MessageBox.Show(ex.Message);
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;

                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                    SqlCmd.Clear();
                    GC.Collect();
                }
            }
            return false;
        }

        // Run_Transaction_Non_Query using List of CMD  
        public bool Run_Transaction_Non_Query(ref List<SqlCommand> SqlCmd, bool throws = false)
        {
            // check connected
            if (Is_Connected())
            {
                // Open Connection First
                Open_Con();
                // Initial Transaction
                SqlTransaction Sqltransaction = SqlCon.BeginTransaction();

                try
                {
                    foreach (SqlCommand Cmd in SqlCmd)
                    {
                        Cmd.Transaction = Sqltransaction;
                        Cmd.Connection = SqlCon;
                        using (Cmd)
                        {
                            Cmd.ExecuteNonQuery();
                        };
                    }
                    // Commit if end the Loop     
                    Sqltransaction.Commit();
                    // return true
                    return true;

                }
                catch/* (Exception ex)*/
                {
                    //   MessageBox.Show(ex.Message);
                    Sqltransaction.Rollback();
                    if (throws)
                        throw;
                    return false;
                }
                finally
                {
                    Sqltransaction.Dispose();
                    Close_Con();
                    SqlCmd.Clear();
                    GC.Collect();
                }
            }
            return false;
        }

        // update table with out Direct Query from DGV
        public void UpdateTbl_From_Dgv(string Tbl_Name, DataGridView dgv)
        {
            try
            {
                if (Is_Connected())
                {
                    SqlDataAdapter da = new SqlDataAdapter("select * from " + Tbl_Name, Get_SQL_Con());
                    DataTable dt;
                    dt = (DataTable)dgv.DataSource;
                    da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                    da.Update(dt);
                }
            }
            catch { throw; }
        }


        // update table with out Direct Query from DevExpress DGV
        public void UpdateTbl_From_Dgv(string Tbl_Name, DataGrid dgv)
        {
            try
            {
                if (Is_Connected())
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from " + Tbl_Name, Get_SQL_Con()))
                    {
                        DataTable dt;
                        dt = (DataTable)dgv.DataSource;
                        da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                        da.Update(dt);
                    }
            }
            catch (SqlException e)
            {
                throw new Exception("SqlException" + e.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception" + ex.Message);
            }

        }






        public void UpdateTbl_From_DataTable(string Tbl_Name, DataTable DGV_DT)
        {
            try
            {
                if (Is_Connected())
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from " + Tbl_Name, Get_SQL_Con()))
                    {
                        DataTable dt;
                        dt = DGV_DT;
                        da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                        da.Update(dt);
                    }
            }
            catch (SqlException /*e*/ )
            {

                //   XtraMessageBox.Show(UserLookAndFeel.Default, "Duplicated Data Please change it ");
                // GridView v =    dgv.DefaultView.GridControl.MainView as GridView;
                // v.OptionsBehavior.AllowValidationErrors = true;

                //  throw new Exception("Data Error");
            }
            //catch (Exception /*ex*/)
            //{
            //    XtraMessageBox.Show(UserLookAndFeel.Default, "Error While Updating data");
            //}

        }

        public void UpdateTbl_From_DataTable(string Tbl_Name, ref DataTable DGV_DT)
        {
            try
            {
                if (Is_Connected())
                    using (SqlDataAdapter da = new SqlDataAdapter("select * from " + Tbl_Name, Get_SQL_Con()))
                    {
                        da.UpdateCommand = new SqlCommandBuilder(da).GetUpdateCommand();
                        da.Update(DGV_DT);
                    }
            }
            catch (SqlException /*e*/ )
            {

                //   XtraMessageBox.Show(UserLookAndFeel.Default, "Duplicated Data Please change it ");
                // GridView v =    dgv.DefaultView.GridControl.MainView as GridView;
                // v.OptionsBehavior.AllowValidationErrors = true;

                //  throw new Exception("Data Error");
            }
            //catch (Exception /*ex*/)
            //{
            //    XtraMessageBox.Show(UserLookAndFeel.Default, "Error While Updating data");
            //}

        }







        // Get Value By Name 
        public int Get_Val_By_Name(string ColID, string TblName, string ColName, string Resource)
        {
            int NO = 0;
            if (Is_Connected())
                using (SqlCommand cmd = new SqlCommand
                    ("Select " + ColID + " From " + TblName + " Where " + ColName + "= N'" + Resource + "'", SqlCon))
                {
                    Open_Con();
                    NO = (int)cmd.ExecuteScalar();
                    Close_Con();
                }
            return NO;
        }

        // Get Name By Value
        public string Get_Name_By_Val(string ColName, string TblName, string ColID, int Resource)
        {
            string Name = string.Empty;
            if (Is_Connected())
                using (SqlCommand cmd = new SqlCommand
                ("Select " + ColName + " From " + TblName + " Where " + ColID + "= " + Resource, SqlCon))
                {
                    Open_Con();
                    Name = (string)cmd.ExecuteScalar();
                    Close_Con();
                }
            return Name;
        }




    }
}
