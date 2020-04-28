using Accord;
using Accord.Statistics.Testing;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_1._1
{
    class Program
    {
        private static string host = "localhost";
        private static string username = "postgres";
        private static string password = "inubenja19113";
        private static string database = "metodosEstadisticos";
        private static NpgsqlConnection conn;

        public static double[] ruralArray = new double[] { };
        public static double[] urbanArray = new double[] { };
        public static double[] oneArray = new double[] { };

        public static double[] ruralAux = new double[20000] ;
        public static double[] urbanAux = new double[20000] ;
        public static double[] oneAux = new double[20000] ;


        static public void ConnectDB()
        {
            var connString = "Host=" + host + ";" + "Username=" + username + ";" +
                    "Password=" + password + ";" + "Database=" + database;

            conn = new NpgsqlConnection(connString);
            conn.Open();
        }
        static private bool isConnected()
        {
            if (conn != null)
            {
                return true;
            }

            return false;
        }
        static public void CloseDB()
        {
            conn.Close();
            conn = null;
        }

        static public void MakeQuerieDB(string input)
        {
            if (isConnected())
            {
                string commandline = "SELECT (t1.Aprobados/t1.Total) as P_Aprobados," +
                                        " (t1.Reprobados / t1.Total) as P_Reprobados," +
                                        " t1.Total" +
                                        " FROM(SELECT(COALESCE(APR_HOM_TO, 0) + COALESCE(APR_MUJ_TO, 0) + COALESCE(APR_SI_TO, 0)) as Aprobados," +
                                        " (COALESCE(REP_HOM_TO, 0) + COALESCE(REP_MUJ_TO, 0) + COALESCE(REP_SI_TO, 0)) as Reprobados," +
                                        " ((COALESCE(APR_HOM_TO, 0) + COALESCE(APR_MUJ_TO, 0) + COALESCE(APR_SI_TO, 0) +" +
                                        " (COALESCE(REP_HOM_TO, 0) + COALESCE(REP_MUJ_TO, 0) + COALESCE(REP_SI_TO, 0)))) as TOTAL" +
                                        " FROM rendimiento_2019";
                if (input == "rural")
                {
                    commandline = commandline + " WHERE COD_DEPE2 = 2" + ") t1";
                }
                else if (input == "urbano")
                {
                    commandline = commandline + " WHERE COD_DEPE2 = 1" + ") t1";
                }
                else
                {
                    Console.WriteLine("Error input");
                    System.Environment.Exit(1);
                }
                commandline = commandline + " WHERE t1.total != 0";
                using var commandDB = new NpgsqlCommand(commandline, conn);
                using NpgsqlDataReader reader = commandDB.ExecuteReader();

                RetrieveData(input, reader);
                
            }
            else
            {
                Console.WriteLine("DB not connected");
                System.Environment.Exit(1);
            }
        }

        static void RetrieveData(string input, NpgsqlDataReader reader)
        {
            int i = 0;
            while (reader.Read())
            {

                if (input == "rural")
                {
                    ruralAux[i] = reader.GetDouble(0);
                    
                    if (i < ruralAux.Length)
                    {
                        // Create new, smaller, array to hold the items we processed.
                        ruralArray = new double[i];
                        Array.Copy(ruralAux, ruralArray, i);
                    }

                }
                else if (input == "urbano")
                {
                    urbanAux[i] = reader.GetDouble(0);
                    if (i < urbanAux.Length)
                    {
                        // Create new, smaller, array to hold the items we processed.
                        urbanArray = new double[i];
                        Array.Copy(urbanAux, urbanArray, i);
                    }
                }
                oneAux[i] = 1;
                oneArray = new double[i];
                Array.Copy(oneAux, oneArray, i);
                i++;

            }
        }

        static double GetMedia(double[] dArray)
        {
            double expectedValue;
            double total = 0;
            for (int i = 0; i < dArray.Length; i++)
            {
                total += dArray[i];
            }
            expectedValue = total / dArray.Length;

            return expectedValue;
        }
        static double GetMediana(double[] dArray)
        {
            double medianaValue;
            int posMedia;
            Array.Sort(dArray);
            if (dArray.Length % 2 == 1)
            {
                posMedia = dArray.Length / 2;
            }
            else posMedia = dArray.Length / 2;

            medianaValue = dArray[posMedia];

            return medianaValue;
        }
        static double GetModa(double[] dArray)
        {
            double modaValue = new double();
            

            return modaValue;
        }

        static double GetVarianza(double[] dArray)
        {
            double varianceValue;
            double media = GetMedia(dArray);

            double total = 0;
            for (int i = 0; i < dArray.Length; i++)
            {
                total += Math.Pow((dArray[i] - media),2);
            }
            varianceValue = total / (dArray.Length -1);

            return varianceValue;
        }
        static double GetDesviacionEstandar(double[] dArray)
        {
            double desviacionValue;
            double varianzaValue = GetVarianza(dArray);

            desviacionValue = Math.Sqrt(varianzaValue);
        

            return desviacionValue;
        }


        static void Main(string[] args)
        {
            ConnectDB();

            Console.WriteLine("Para rural:");
            double expectedValueRural;
            double varianceValueRural;
            double desviacionValueRural;


            MakeQuerieDB("rural");
            expectedValueRural = GetMedia(ruralArray);
            varianceValueRural = GetVarianza(ruralArray);
            desviacionValueRural = GetDesviacionEstandar(ruralArray);

            Console.WriteLine("n: " + ruralArray.Length.ToString());

            Console.WriteLine("esperanza rural: " + expectedValueRural.ToString());
            Console.WriteLine("varianza rural: " + varianceValueRural.ToString());
            Console.WriteLine("desviacion rural: " + desviacionValueRural.ToString());

            Console.WriteLine();

            Console.WriteLine("Para urbano:");
            double expectedValueUrbano;
            double varianceValueUrbano;
            double desviacionValueUrbano;

            MakeQuerieDB("urbano");

            expectedValueUrbano = GetMedia(urbanArray);
            varianceValueUrbano = GetVarianza(urbanArray);
            desviacionValueUrbano = GetDesviacionEstandar(urbanArray);

            Console.WriteLine("n: " + urbanArray.Length.ToString());

            Console.WriteLine("esperanza urbana: " + expectedValueUrbano.ToString());
            Console.WriteLine("varianza urbana: " + varianceValueUrbano.ToString());
            Console.WriteLine("desviacion urbana: " + desviacionValueUrbano.ToString());



          


            CloseDB();
            Console.WriteLine("FINISHED");
        }

        
    }
}
