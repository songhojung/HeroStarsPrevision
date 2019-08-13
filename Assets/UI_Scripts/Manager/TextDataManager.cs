using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TextDataManager
{
	//모든 언어관련 텍스트데이터 담을 컬렉션
	public static Dictionary<LanguageCode, Dictionary<int, string>> Dic_AllTranslateText = new Dictionary<LanguageCode, Dictionary<int, string>>();
	//모든 언어관련 에러텍스트데이터 담을 컬렉션
	public static Dictionary<LanguageCode, Dictionary<int, string>> Dic_AllErrorTranslateText = new Dictionary<LanguageCode, Dictionary<int, string>>();
	public static Dictionary<int, string> Dic_TranslateText = new Dictionary<int, string>(); // 선택된 언어 텍스트데이터 담을 컬렉션
	public static Dictionary<string, int> Dic_Kr_TraslateTxt = new Dictionary<string, int>(); // 한국어만 담은 텍스트 데이터
	public static Dictionary<int, string> Dic_ErrorCode = new Dictionary<int, string>(); // 에러 코드만 담은 텍스트 데이터
	public static Dictionary<string, bool> Dic_BannedText = new Dictionary<string, bool>(); // 금칙 단어를 담은 텍스트데이터
	public static Dictionary<char, bool> Dic_BannedChar = new Dictionary<char, bool>(); // 금칙어를 담은 텍스트데이터

	public static List<LanguageCode> Lst_language = new List<LanguageCode>();

	public static int count;
	public static int TypeNum;

	public static void clear()
	{
		Dic_TranslateText.Clear();
	}

	public static void GetTextData(LanguageCode _lang)
	{
		if (Dic_AllTranslateText.Count <= 0)
		{
			ReadAllTextData();
			Dic_TranslateText = Dic_AllTranslateText[_lang];
			Dic_ErrorCode = Dic_AllErrorTranslateText[_lang];

			LanguageManager.Getsingleton.SetLanguageRefresh();

			Load_ChatFilterData();
		}
		else
		{
			Dic_TranslateText = Dic_AllTranslateText[_lang];
			Dic_ErrorCode = Dic_AllErrorTranslateText[_lang];
		}



		// speech txt 정보 담기 
		var speechTxts = from txts in Dic_TranslateText
						 where txts.Key >= 10000 && txts.Key < 20000
						 select txts;

		count = speechTxts.Count();
		TypeNum = Enum.GetValues(typeof(SpeechTxtType)).Length - 1;

	}

	public static void ReadAllTextData()
	{
		Array langArray =  Enum.GetValues(typeof(LanguageCode));
		for (int i = 0; i < langArray.Length; i++ )
		{
			LanguageCode lang = (LanguageCode)langArray.GetValue(i);
			if (lang == LanguageCode.NONE)
				continue;
			else
			{
				Dic_AllTranslateText[lang] = ReadTextData(lang);
				Dic_AllErrorTranslateText[lang] = ReadErrorTextData(lang);
			}
		}
	}


	public static Dictionary<int, string> ReadTextData(LanguageCode _lang)
	{


		// 지역 transtext 컬랙션
		Dictionary<int, string> _dic_TranslateText = new Dictionary<int, string>();

		string _path = string.Format("TextData/{0}/{1}", _lang.ToString(), "TextData");
		TextAsset _txtAset = Resources.Load(_path) as TextAsset;

		// LanguageCOde 정의했지만 데이터가 없는 경우 리턴 떄리자
		if (_txtAset == null)
			return _dic_TranslateText;

		string _jsonData = _txtAset.text;

		object _jsonObj = MiniJSON.Json.Deserialize(_jsonData); // 라이브러리 json 파서
		List<object> _lstData = (List<object>)_jsonObj;


#if UNITY_EDITOR
		if (_lstData.Count == 0 || _lstData == null)
		{
			Debug.LogError("TEXT DATA IS NULL OR EMPTY");
		}
#endif
		int _idx = 0;
		
		for (int i = 0; i < _lstData.Count; i++)
		{
			Dictionary<string, object> _dicData = (Dictionary<string, object>)_lstData[i];

			_idx = Convert.ToInt32(_dicData[JsonKey.IDX]);
			
			string _text = Convert.ToString(_dicData[JsonKey.TEXT]);

			//정보담기
			_dic_TranslateText[_idx] = _text;


			/*int _mainKnd = _idx >> 7; // 인덱스 번호를 2진수 비트연산으로 대분류 번호를 추출
			int _subKnd = (_idx & ((1 << 7) - 1));// ((1<<7)-1) 클리어라 생각... idx 추출
			
			switch (_mainKnd)
			{
				case 0: TimeText[_subKnd] = _text; break;
				case 1: ItTpText[_subKnd] = _text; break;
			}
			*/

			//switch (_TrText)
			//{
			//    case TRANSLATE_TEXT.Combat: Dic_TranslateText[_TrText] = _text; break;
			//    case TRANSLATE_TEXT.Store: Dic_TranslateText[_TrText] = _text; break;
			//    case TRANSLATE_TEXT.Clan: Dic_TranslateText[_TrText] = _text; break;
			//    case TRANSLATE_TEXT.Formation: Dic_TranslateText[_TrText] = _text; break;
			//}


		} // end of for


		//반환
		return _dic_TranslateText;
	}

	

	public static Dictionary<int, string> ReadErrorTextData(LanguageCode lang)
	{
		Dictionary<int, string> _dic_ErrorTranslateText = new Dictionary<int, string>();

		TextAsset _txtAsset = Resources.Load(string.Format("TextData/{0}/{1}",lang.ToString(),"ErrorCode")) as TextAsset;

		// LanguageCOde 정의했지만 해당언어 데이터가 없는 경우 리턴 떄리자
		if (_txtAsset == null)
			return _dic_ErrorTranslateText;
		
		string _jsonData = _txtAsset.text;


		object _jsonObj = MiniJSON.Json.Deserialize(_jsonData);
		List<object> _lstData = (List<object>)_jsonObj;

#if UNITY_EDITOR
		if (_lstData.Count == 0 || _lstData == null)
		{
			Debug.LogError("TEXT DATA IS NULL OR EMPTY");
		}
#endif
		int _idx = 0;

		for (int i = 0; i < _lstData.Count; i++)
		{
			Dictionary<string, object> _dicData = (Dictionary<string, object>)_lstData[i];
			_idx = Convert.ToInt16(_dicData[JsonKey.idx]);
			_dic_ErrorTranslateText[_idx] = Convert.ToString(_dicData[JsonKey.TEXT]);
		}

		return _dic_ErrorTranslateText;
	}


	public static void Load_ChatFilterData()
	{
		string _assetText = (Resources.Load("TextData/BannedTextList") as TextAsset).text;
		string[] filterTexts = _assetText.Split(',');

		for (int i = 0; i < filterTexts.Length; i++ )
		{
			//금칙 단어들 담자
			Dic_BannedText[filterTexts[i]] = true;


			//금칙어 하나씩담기
			char[] _chars = filterTexts[i].ToCharArray();

			for (int h = 0; h < _chars.Length; h++ )
			{
				if (!Dic_BannedChar.ContainsKey(_chars[h]))
					Dic_BannedChar[_chars[h]] = true;
			}
			
		}





		//string txt1 = "사랑대가빡";
		//string txt2 = "대가빡사랑";
		//string txt3 = "니미 아나 머하냐 시발";
		//string txt4 = "아가리 똥 박았냐 닝기미";
		//
		//Chk_BannedLetter(ref txt1);
		//Chk_BannedLetter(ref txt2);
		//Chk_BannedLetter(ref txt3);
		//Chk_BannedLetter(ref txt4);
		//
		//Debug.Log(txt1);
		//Debug.Log(txt2);
		//Debug.Log(txt3);
		//Debug.Log(txt4);
		
	}
	const string coverTxt1 = "*";
	const string coverTxt2 = "**";
	const string coverTxt3 = "***";

	//한단어씩을 체크
	public static bool Chk_BannedWord(ref string inputTxt)
	{
		bool isBanTxt = false;

		string[] _words = inputTxt.Split(' ');

		for (int i = 0; i < _words.Length; i++ )
		{
			if (Dic_BannedText.ContainsKey(_words[i]))
			{
				isBanTxt = true;
				_words[i] = _words[i].Length <= 2 ? coverTxt2 : coverTxt3;
			}
		}

		if (isBanTxt)
		{
			inputTxt = string.Empty;

			for (int i = 0; i < _words.Length; i++ )
			{
				inputTxt += string.Format("{0} ", _words[i]);
			}
		}

		return isBanTxt;
	}


	// 한 글자씩체크
	public static bool Chk_BannedLetter(ref string inputTxt)
	{
		bool isBantxt = false;
		//bool isDoubtLetter = false; // 현재 글자가 욕인지 의심스럽냐;
		string[] splitTxt = inputTxt.Split(' ');


		if (Chk_BannedWord(ref inputTxt))
			isBantxt = true;

		if (!isBantxt)
		{


			for (int g = 0; g < splitTxt.Length; g++)
			{
				char[] _char = splitTxt[g].ToCharArray();
				string chkedTxt = string.Empty;
				string doubtStr = string.Empty;
				bool chkspellStart = false;

				for (int i = 0; i < _char.Length; i++)
				{
					if (Dic_BannedChar.ContainsKey(_char[i]))
					{
						if (chkspellStart)
						{
							// 바로 금칙처리

							if (Dic_BannedText.ContainsKey(doubtStr))
							{
								isBantxt = true;
								doubtStr = doubtStr.Length <= 2 ? coverTxt2 : coverTxt3;
								chkedTxt += doubtStr;
								doubtStr = string.Empty;

								continue;
							}
						}
						else if(!chkspellStart)
						{
							doubtStr += _char[i].ToString();
						}


						if (doubtStr.Length >= 2) //두글자이상이니 검사하자
						{

							if (Dic_BannedText.ContainsKey(doubtStr))
							{
								chkspellStart = true;

							}
							else
							{
								chkspellStart = false;
							}


							//if (chkspellStart == false)
							//{
							//    string removeDoubtStr = doubtStr.Remove(doubtStr.Length - 1); // 마지막글자 뺴자

							//    if (Dic_BannedText.ContainsKey(doubtStr))
							//    {
							//        isBantxt = true;
							//        doubtStr = doubtStr.Length <= 2 ? coverTxt2 : coverTxt3;
							//    }
							//    else if (Dic_BannedText.ContainsKey(removeDoubtStr))
							//    {
							//        isBantxt = true;

							//        removeDoubtStr = removeDoubtStr.Length <= 2 ? coverTxt2 : coverTxt3;
							//        doubtStr = removeDoubtStr + _char[i];
							//    }
							//    else
							//    {
							//        chkedTxt += doubtStr;
							//        doubtStr = string.Empty;
							//    }
							//}

						}

					}
					else
					{

						if (!string.IsNullOrEmpty(doubtStr) && Dic_BannedText.ContainsKey(doubtStr))
						{
							isBantxt = true;
							doubtStr = doubtStr.Length <= 2 ? coverTxt2 : coverTxt3;
							chkedTxt += doubtStr + _char[i];
							doubtStr = string.Empty;

						}
						else
						{
							chkedTxt += doubtStr + _char[i];
							doubtStr = string.Empty;
							chkspellStart = false;
						}
					}

				}//endof char for

				if (chkspellStart)
				{
					if (Dic_BannedText.ContainsKey(doubtStr))
					{
						isBantxt = true;
						doubtStr = doubtStr.Length <= 2 ? coverTxt2 : coverTxt3;
					}
				}

				splitTxt[g] = chkedTxt + doubtStr;
			}//endof splitTxt for


			if (isBantxt)
			{
				inputTxt = string.Empty;

				for (int i = 0; i < splitTxt.Length; i++)
				{
					inputTxt += string.Format("{0} ", splitTxt[i]);
				}
			}

		} //end of isbanText if 
		return isBantxt;
	}


	
	public static string Get_SpeechTxt(SpeechTxtType speechType)
	{
		int type = (int)speechType;

		int speechIdx = 10000 + (type * 10) + UnityEngine.Random.Range(1, (count / TypeNum )+ 1);

		return Dic_TranslateText[speechIdx];
	}
}
