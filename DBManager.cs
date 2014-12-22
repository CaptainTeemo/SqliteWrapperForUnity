using UnityEngine;
using System;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class DBManager : MonoBehaviour {
	static private SqliteConnection _dbConnection;
	static private SqliteCommand _dbCommand;
	static private SqliteDataReader _reader;

	private static DBManager _sharedDBManager = null;

	public static DBManager sharedManager() {
		if (_sharedDBManager == null) {
			GameObject obj = new GameObject();
			_sharedDBManager = obj.AddComponent<DBManager>();
		}
		return _sharedDBManager;
	}

	public DBManager () {

	}
	
	public void openDB (string fileName) {
		string createString;
		string appDBPath = Application.persistentDataPath + "/" + fileName;
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			createString = @"Data Source=" + appDBPath;
		} else if (Application.platform ==  RuntimePlatform.Android) {
			createString = "URI=file:" + appDBPath;
		} else {
			Debug.Log ("editor");
			createString = "data source = " + Application.streamingAssetsPath + "/" + fileName;
		}
//		yield return StartCoroutine(copyFileToDevice(fileName));

		//establish database connection
		try {
			_dbConnection = new SqliteConnection(createString);
			_dbConnection.Open();
			Debug.Log("connected to db");
		} catch (Exception e){
			string exceptionString = e.ToString();
			Debug.Log(exceptionString);
		}
	}

	public IEnumerator copyFileToDevice(string fileName) {
		string appDBPath = Application.persistentDataPath + "/" + fileName;
		if (!File.Exists(appDBPath)) {
			string prefix;
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				prefix = "file://";
			} else {
				prefix = "";
			}
			WWW loadDB = new WWW(prefix + Application.streamingAssetsPath + "/" + fileName);
			Debug.Log(prefix + Application.streamingAssetsPath + "/" + fileName);
			yield return loadDB;
			File.WriteAllBytes(appDBPath, loadDB.bytes);
			Debug.Log("copy file to local dir");
		}
	}
	
	public void closeConnection () {
		if (_dbCommand != null) {
			_dbCommand.Dispose();
		}
		_dbCommand = null;
		
		if (_reader != null) {
			_reader.Close();
		}
		_reader = null;
		
		if (_dbConnection != null) {
			_dbConnection.Dispose();
		}
		_dbConnection = null;
		Debug.Log("Disconnected from db");
	}
	
	public SqliteDataReader executeQuery (string query) {
		Debug.Log("sql: " + query);
		_dbCommand = _dbConnection.CreateCommand();
		_dbCommand.CommandText = query;
		_reader = _dbCommand.ExecuteReader();
		return _reader;
	}

	//check table exists or not in sqlite database
	public bool CheckTableExists(string tableName) {
		string query = "select count(type) from sqlite_master where type='table' and name ='" + tableName + "'";
		SqliteDataReader reader = this.executeQuery(query);
		return reader.HasRows;
	}
}
