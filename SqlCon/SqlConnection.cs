using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SqlCon
{
    public class SqlConnection
    {
        readonly System.Data.SqlClient.SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection();
      


        public SqlConnection(string Connectionstring) => Get_SQl_Con(Connectionstring);


        // read connection string from string will come from Cls_Read_Con
        private void Get_SQl_Con(string Con_string) => SqlCon.ConnectionString = Con_string;



        // return SqlConnection maybe i need it 
        public System.Data.SqlClient.SqlConnection Get_SQL_Con() { if (Is_Connected()) { return SqlCon; } return null; }

        // if connection close open it
        private void Open_Con()
        { if (SqlCon.State == ConnectionState.Closed) SqlCon.Open(); }

        //if connection open close it
        private void Close_Con()
        { if (SqlCon.State == ConnectionState.Open) SqlCon.Close(); }

        // boolean for check if connection staple , Thrown new exmircc
        private bool Is_Connected()
        {
            try
            {
                Open_Con();
                Close_Con();
                return true;
            }
            catch
            {
                SqlCon.Dispose();
                return false;
                throw new Exception("Can't connect to Database !!!");
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
            return 0;
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
                    { da.Fill(dt); };
            }
            catch 
            {
                throw;
            }

            return dt;

        }


        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(string Select_Query)
        {
            DataTable dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && Select_Query.Length > 1)
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(Select_Query, SqlCon))
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


        // Is Row Duplicated!!??
        public bool IS_Data_Duplicated(string Select_Query, ref DataTable dt)
        {
            dt = new DataTable(); dt.Rows.Clear();
            try
            {
                if (Is_Connected() && Select_Query.Length > 1)
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(Select_Query, SqlCon))
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



        // Run_Non_Query using string 
        public bool Run_Non_Query(string Cmd_String)
        {
            try
            {
                if (Is_Connected())
                {

                    using (SqlCommand SqlCmd = new SqlCommand(Cmd_String, SqlCon))
                    {
                        Open_Con();
                        SqlCmd.ExecuteNonQuery();
                        Close_Con();
                        return true;
                    }
                }
            }
             catch 
            {

                throw;
            }
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
             catch 
            {
                throw;
            }
            return false;
        }

        // Run_Transaction_Non_Query using string Cmd
        public bool Run_Transaction_Non_Query(string Cmd_String)
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

        // Run_Transaction_Non_Query using CMD  
        public bool Run_Transaction_Non_Query(ref SqlCommand SqlCmd)
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

        // Run_Transaction_Non_Query using List of CMD  
        public bool Run_Transaction_Non_Query(ref List<SqlCommand> SqlCmd)
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
             catch 
            {
                throw;
            }
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


        // Get Value By Name 
        public int Get_Val_By_Name(string ColID, string TblName, string ColName, string Resource)
        {
            int NO = 0;
            if (Is_Connected())

                using (SqlCommand cmd = new SqlCommand
                    ("Select " + ColID + " From " + TblName + " Where " + ColName + "= '" + Resource + "'", SqlCon))
                {
                    Open_Con();
                    NO = (int)cmd.ExecuteScalar();
                    Close_Con();
                }
            return NO;
        }

        // Get Name By Value
        public string Get_Name_By_Val(string ColName, string TblName, string ColID, string Resource)
        {
            string Name = string.Empty;
            if (Is_Connected())
                using (SqlCommand cmd = new SqlCommand
                ("Select " + ColName + " From " + TblName + " Where " + ColID + "= " + Resource + "", SqlCon))
                {
                    Open_Con();
                    Name = (string)cmd.ExecuteScalar();
                    Close_Con();
                }
            return Name;
        }



    }
}
