﻿#region Copyright Notice & License Information
// 
// INI.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hydrogen.Serialization
{
	public class INI
	{

		public static Dictionary<string, string> Deserialize(string iniString, char seperatorCharacter = '=')
		{
			Dictionary<string, string> entries = new Dictionary<string, string>();
			using (StringReader reader = new StringReader(iniString))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					line = line.Trim();
					if (line.Length == 0)
						continue;
					string[] s = line.Split(new char[]{seperatorCharacter}, 2);
					
					if (s.Length != 2)
						continue;
					
					entries.Add(s[0].Trim(), s[1].Trim());
				}
			}
			return entries;
		}

		public static string Serialize(Dictionary<string, string> data, char seperatorCharacter = '=')
		{
			StringBuilder iniString = new StringBuilder();

			foreach(string s in data.Keys)
			{
				iniString.AppendLine(s.Trim() + seperatorCharacter + data[s].Trim());
			}

			return iniString.ToString();
		}
	}
}