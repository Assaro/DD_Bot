/* DD_Bot - A Discord Bot to control Docker containers*/

/*  Copyright (C) 2022 Maxim Kovac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

*/

using System.IO;
using DD_Bot.Domain;
using Newtonsoft.Json;

namespace DD_Bot.Application.Providers
{
    public class LanguageProvider
    {
        public static void CreateStandardLanguage(string languageFile)
        {
            var language = new Language();
            File.WriteAllText(languageFile, JsonConvert.SerializeObject(language));
        }
    }
}
