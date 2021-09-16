using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
public class DataAccess {

    string filepath;
    string conn;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string sqlQuery;
    int num;
 
	public DataAccess()
    {
        filepath = Application.persistentDataPath + "/mobilegameDB.db";
        
        if (!File.Exists(filepath))
        {
            WWW loadDB;
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                loadDB = new WWW(Application.dataPath + "/StreamingAssets/mobilegameDB.db");  // this is the path to your StreamingAssets in pc
            }
            else
            {
                // if it doesn't ->

                // open StreamingAssets directory and load the db ->

                loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/mobilegameDB.db");  // this is the path to your StreamingAssets in android
            }
            while (!loadDB.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check

            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDB.bytes);
        }

        conn = "URI=file:" + filepath;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        num = 0;
    }
    public int getMaxData()
    {
        sqlQuery = "SELECT max(user_id) AS max from user_data";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            if (reader["max"] != System.DBNull.Value)
                num = Convert.ToInt32(reader["max"]);
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return num;
    }
    public void saveDataToDB (string query)
    {
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        dbconn.Close();
    }
    public void getWords (ref int word_id, ref int trivia_id, ref string imageFP, ref int sound_id, ref string word, string query)
    {
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            word_id = Convert.ToInt32(reader["word_id"]);
            word = reader["word"].ToString();
            imageFP = reader["image"].ToString();
            sound_id = Convert.ToInt32(reader["sound_id"]);
            trivia_id = Convert.ToInt32(reader["trivia_id"]);
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
    }
    
    public char[] getExtraLetters(int limit)
    {
        char[] letters=new char[limit];
        int i = 0;
        sqlQuery = "SELECT letter FROM alphabet order by random() limit " + limit;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            letters[i] = Convert.ToChar(reader["letter"]);
            i++;
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return letters;
    }

    public int getNum (string query, string col)
    {
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            if (reader[col] != System.DBNull.Value)
                num = Convert.ToInt32(reader[col]);
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return num;
    }
    public string getTrivia (int trivia_id)
    {
        string triv="";
        sqlQuery = "SELECT trivia FROM trivia WHERE trivia_id=" + trivia_id;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            triv = reader["trivia"].ToString();
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return triv;
    }
    public string getString(string query, string column)
    {
        string str = "";
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            str = reader[column].ToString();
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return str;
    }
    public bool dataExist(string query, string column)
    {
        bool ret = false;
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            if (reader[column] != System.DBNull.Value)
                ret = true;
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return ret;
    }
    public string[] getStringArray(int limit, string query, string column)
    {
        string[] arr = new string[limit];
        int i = 0;
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            if (i<limit)
            {
                arr[i] = reader[column].ToString();
                i++;
            }
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
        return arr;
    }
    public void getWordsArray(ref int[] word_id, ref int[] def_id, ref string[] imageFP, ref int[] sound_id, ref string[] word, string query)
    {
        int i = 0;
        sqlQuery = query;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            word_id[i] = Convert.ToInt32(reader["word_id"]);
            word[i] = reader["word"].ToString();
            imageFP[i] = reader["image"].ToString();
            sound_id[i] = Convert.ToInt32(reader["sound_id"]);
            def_id[i] = Convert.ToInt32(reader["def_id"]);
            i++;
        }
        reader.Close();
        dbcmd.Dispose();
        dbconn.Close();
    }
}
