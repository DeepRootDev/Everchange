using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

///////////////////////////////////////////////////////////////
// saves a .csv (comma separated values) file
// which is excel and google docs compatible!
// so we can make cool graphs with the results 
///////////////////////////////////////////////////////////////
public class SpeedrunStats : MonoBehaviour
{
    // safe for web builds:
    [Header("Check this to not create any files:")]
    public bool usePlayerPrefs = true;

    // file to save in your user documents folder
    // only used if usePlayerPrefsNotFiles is false
    [Header("Filename to save data to:")]
    public string dbFilename = "Everchange-results.csv";

    // where to draw the timer and scoreboard
    [Header("Which GUI text component to use?")]
    public TextMeshProUGUI timerTXT;

    // just for testing
    [Header("Start immediately and save with T key?")]
    public bool debug_mode = true;

    // the database file is a .csv located in a folder we're allowed to write to
    // it is a text file with rows on each new line with data separated by commas
    private string dbFilepath;
    private char rowSeparator = '\n';
    private char colSeparator = ',';

    // the data from disk once parsed so we can sort etc
    // stored as rows and columns of strings
    private List<List<string>> databaseData = null;

    // the current run start time!
    private float startTimestamp = 0f;

    ///////////////////////////////////////////////////////////////
    void Start()
    {
        dbFilepath = Application.persistentDataPath + "/" + dbFilename;
        csvReadFile();
        // JUST FOR DEBUGGING: start the timer right on game start!
        if (debug_mode) startSpeedrun();
    }

    ///////////////////////////////////////////////////////////////
    public void startSpeedrun()
    {
        Debug.Log("Starting the speedrun stopwatch NOW.");
        startTimestamp = Time.time; // always 0 at this point
    }

    ///////////////////////////////////////////////////////////////
    public void endSpeedrun()
    {
        float elapsed = Time.time - startTimestamp;
        Debug.Log("speedrun was ended by player!");
        // add time to the log
        System.DateTime theTime = System.DateTime.Now;
        string stamp =
            theTime.Year + "-" + pad0(theTime.Month) + "-" + pad0(theTime.Day) + " " +
            pad0(theTime.Hour) + ":" + pad0(theTime.Minute) + ":" + pad0(theTime.Second);
        csvAppendRow(new List<string> { stamp, timespanFormat(elapsed) });
        // reset the timer
        startTimestamp = Time.time;
        // fixme: respawn player
    }

    ///////////////////////////////////////////////////////////////
    // force a number to be two digits like "01"
    string pad0(int x)
    {
        if (x < 10) return "0" + x;
        return "" + x;
    }

    ///////////////////////////////////////////////////////////////
    // turn seconds into a string like "1h 30m 45s 50ms"
    string timespanFormat(float seconds)
    {
        string str = "";
        int mm = Mathf.FloorToInt(seconds / 60f);
        int ss = Mathf.FloorToInt(seconds - mm * 60);
        int ms = Mathf.FloorToInt(Mathf.Repeat(seconds, 1f) * 1000f);
        if (mm < 10) str += "0";
        str += mm + "m ";
        if (ss < 10) str += "0";
        str += ss + "s ";
        if (ms < 10) str += "00";
        else if (ms < 100) str += "0";
        str += ms + "ms";
        return str;
    }

    ///////////////////////////////////////////////////////////////
    void csvReadFile()
    {
        Debug.Log("Reading speedrun stats data file: " + dbFilepath);
        databaseData = ReadData(dbFilepath);
        Debug.Log("csv loaded with " + databaseData.Count + " rows of data!");
        /* example of how to parse:
        foreach (List<string> row in databaseData)
        {
            foreach (string field in row)
            {
                // contentArea.text += field + "\t";
            }
        } */

    }

    ///////////////////////////////////////////////////////////////
    public void csvAppendRow(List<string> rowData)
    {
        Debug.Log("Adding a new row of stats to: " + dbFilepath);
        AddData(dbFilepath, rowData);
        csvReadFile(); // fixme: unneccessary file access - just add a row to databaseData
    }

    ///////////////////////////////////////////////////////////////
    public List<List<string>> ReadData(string filename)
    {
        List<List<string>> result = new List<List<string>>();
        try
        {

            string fileContents;

            if (usePlayerPrefs)
            {
                // we use the "filename" as the prefs key name
                fileContents = PlayerPrefs.GetString(filename, "");
            }
            else
            {
                // FILE VERSION
                var source = new StreamReader(filename);
                fileContents = source.ReadToEnd();
                source.Close();
            }

            var records = fileContents.Split(rowSeparator);
            foreach (string record in records)
            {
                List<string> row = new List<string>();
                string[] fields = record.Split(colSeparator);
                foreach (string field in fields)
                {
                    // remove any quotes or trailing or leading whitespace
                    string cleanedString = field.Trim().Trim('\"'); 
                    row.Add(cleanedString);
                }
                result.Add(row);
            }
        }
        catch (Exception /* ex */)
        {
            Debug.LogError("ERROR trying to read this file: " + filename);
        }
        return result;
    }

    ///////////////////////////////////////////////////////////////
    public void AddData(string filename, List<string> values)
    {
        try
        {
            string data = rowSeparator.ToString();
            foreach (string value in values)
            {
                // for strings to import nicely, add quotes around them
                data += "\"" + value + "\"" + colSeparator;
            }

            if (usePlayerPrefs)
            {
                // use the filename as the prefs key
                string oldData = PlayerPrefs.GetString(filename, "");
                PlayerPrefs.SetString(filename, oldData+data);
            }
            else
            {
                if (!File.Exists(filename))
                {   // make the first row a "header row"
                    Debug.Log("Creating new stats db file: " + filename);
                    File.AppendAllText(filename, "\"DATE:\",\"RESULT:\"");
                }
                File.AppendAllText(filename, data);
            }
        }
        catch (Exception /* ex */)
        {
            Debug.LogError("ERROR trying to write to this file: " + filename);
        }
    }


    ///////////////////////////////////////////////////////////////
    // update the GUI every frame with a stopwatch timer
    void Update()
    {
        if (!timerTXT) return; // no text control to update? then don't bother
        float elapsed = Time.time - startTimestamp;
        string prevstr = "";
        int maxLines = 20; // only show the last few attempts
        if (databaseData != null)
        {
            prevstr += databaseData.Count + " Previous Runs:\n";
            // start at end of data and work backwards
            for (int n = databaseData.Count - 1; n >= Math.Max(0, databaseData.Count - maxLines); n--)
            {
                prevstr += "\n";
                foreach (string field in databaseData[n]) { prevstr += field + " "; }
            }
        }
        timerTXT.text =
            "SPEEDRUN TIMER:\n\n" +
            "Elapsed Time: " + timespanFormat(elapsed) + "\n\n" +
            prevstr +
            "\n\n" + dbFilepath;

        if (debug_mode)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                endSpeedrun(); // save result!!! and reset timer
            }
        }
    }
}
