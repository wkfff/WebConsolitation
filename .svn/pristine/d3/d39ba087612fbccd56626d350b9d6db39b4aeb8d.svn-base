using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    internal partial class SchemeClass : SMOSerializable, IScheme
    {
        /// <summary>
        /// Класс проверки осмысленности комментариев.
        /// </summary>
        private class CommentsCheckService : DisposableObject, ICommentsCheckService
        {
            private readonly List<string> invalidItems;
            private readonly StringElephanterSettings settings;

            /// <summary>
            /// Инициализация экземпляра класса.
            /// </summary>
            internal CommentsCheckService()
            {
                settings = new StringElephanterSettings();
                settings.AllowSingleChars = false;

                invalidItems = new List<string>();
                try
                {
                    using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\CommentsCheckList.txt", Encoding.GetEncoding(1251)))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            try
                            {
                                string processedComment = StringElephanter.TrampDown(line, settings);
                                invalidItems.Add(processedComment);
                            }
                            catch (Exception)
                            {
                                /*Trace.TraceVerbose(e.Message);*/
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    /*Trace.TraceVerbose(e.Message);*/
                }
            }

            /// <summary>
            /// Проверяет текст комментария на осмысленность.
            /// </summary>
            /// <param name="comments">Текст комментария.</param>
            /// <returns>true - комментарий осмысленный.</returns>
            public bool CheckComments(string comments)
            {
                if (invalidItems.Count > 0)
                {
                    if (comments == String.Empty
                        || comments.Length < 6)
                        return false;

                    string processedComment = StringElephanter.TrampDown(comments, settings);
                    
                    return !invalidItems.Contains(processedComment);
                }
                else
                    return comments.Length > 5;
            }
        }

        /// <summary>
        /// Экземпляр класса проверки осмысленности комментариев.
        /// </summary>
        private CommentsCheckService commentsCheckService;

        /// <summary>
        /// Экземпляр класса проверки осмысленности комментариев.
        /// </summary>
        private CommentsCheckService CommentsCheckServiceInstance
        {
            get
            {
                //if (commentsCheckService == null)
                    commentsCheckService = new CommentsCheckService();
                return commentsCheckService;
            }
        }
    }
}
