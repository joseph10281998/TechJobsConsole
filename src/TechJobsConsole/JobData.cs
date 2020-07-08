using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;   //conventional design pattern "that is related to data loading" 

        /* Each line in the CSV file is a Dictionary item in the List,
         * and each of these keys provide a way to lookup the Dictionary elements:
        name,employer,location,position type, core competency
        ... so, given the first line of data,..
         Junior Data Analyst, Lockerdome, Saint Louis,Data Scientist / Business Intelligence, Statistical Analysis
        ... d["name"] would return "Junior Data Analyst"
        */
        /* example: List<string> box = new List<string>()
         *            box.Add("book");
         *            box.Add("pen");
         *           if (box.Contains("pen") {
         *           do something with it
         *          List = {book, pen,....}
         * In this case, we have:  
         *   List All Jobs = {dictionary1,= { name; "Junior Data Analyst", "employer": "Lockerdome", ...}
         *                    dictionary2,= (emplyer
         *                    dictionary3
         *                    ....} - It is like overall, we have 98*5 matrix*/

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. E.g. passing "location" will return a
         * list of distint job locations.
         */
        public static List<string> FindAll(string column)
        {
            column = column.ToLower();    //case-insensitive
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column].ToLower();        //case-insensitive

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            column = column.ToLower();         //case-insensitive
            value = value.ToLower();           //case-insensitive
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column].ToLower();       //case-insensitive

                if (aValue.Contains(value))
                {
                    jobs.Add(row);
                }
            }

            return jobs;
        }


        public static List<Dictionary<string, string>> FindByValue(string searchValue)
        {
            searchValue = searchValue.ToLower();       //case-insensitive
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                foreach (KeyValuePair<string, string> kvp in row)
                {
                    string aValue = kvp.Value.ToLower();  //case-insensitive

                    if (aValue.Contains(searchValue))
                    {
                        jobs.Add(row);
                        break;         //To avoid dupulication of same keywords
                    }
                }
            }

            return jobs;
        }




        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();
            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
