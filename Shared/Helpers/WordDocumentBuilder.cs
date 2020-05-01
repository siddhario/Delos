using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace ServisDB.Klase
{
    public class WordDocumentBuilder
    {
        public static void FillBookmarksUsingOpenXml(string sourceDoc, string destDoc, Dictionary<string, string> bookmarkData)
        {
            string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            // Make a copy of the template file.
            File.Copy(sourceDoc, destDoc, true);

            //Open the document as an Open XML package and extract the main document part.
            using (WordprocessingDocument wordPackage = WordprocessingDocument.Open(destDoc, true))
            {
                MainDocumentPart part = wordPackage.MainDocumentPart;

                //Setup the namespace manager so you can perform XPath queries 
                //to search for bookmarks in the part.
                NameTable nt = new NameTable();
                XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
                nsManager.AddNamespace("w", wordmlNamespace);

                //Load the part's XML into an XmlDocument instance.
                XmlDocument xmlDoc = new XmlDocument(nt);
                xmlDoc.Load(part.GetStream());

                //Iterate through the bookmarks.
                foreach (KeyValuePair<string, string> bookmarkDataVal in bookmarkData)
                {
                    var bookmarks = from bm in part.Document.Body.Descendants<BookmarkStart>()
                                    select bm;

                    foreach (var bookmark in bookmarks)
                    {
                        if (bookmark.Name == bookmarkDataVal.Key)
                        {
                            Run bookmarkText = bookmark.NextSibling<Run>();
                          
                            string[] parts = Regex.Split(bookmarkDataVal.Value, Environment.NewLine);
                            if (bookmarkText != null)  // if the bookmark has text replace it
                            {
                                bookmarkText.GetFirstChild<Text>().Text = bookmarkDataVal.Value;
                            }
                            else  // otherwise append new text immediately after it
                            {
                                var parent = bookmark.Parent;   // bookmark's parent element

                                Paragraph p = new Paragraph();
                                ParagraphProperties pp = new ParagraphProperties();
                                pp.Justification = new Justification() { Val = JustificationValues.Left };
                                // Add paragraph properties to your paragraph
                                p.Append(pp);

                                Text text = new Text(bookmarkDataVal.Value);
                                //string[] parts = Regex.Split(bookmarkDataVal.Value,Environment.NewLine);
                                Run run = new Run(new RunProperties());
                                List<string> rows = parts.Where(pr => pr != "").ToList();
                                foreach (string s in rows)
                                {
                                    run.Append(new Text(s));
                                    run.Append(new Break());
                                }
                                p.Append(run);

                                ((Paragraph)bookmark.Parent).InsertAfterSelf(p);
                                
                                // insert after bookmark parent
                                //parent.Append(run);
                            }

                            //bk.Remove();    // we don't want the bookmark anymore
                        }
                    }
                }

                //Write the changes back to the document part.
                xmlDoc.Save(wordPackage.MainDocumentPart.GetStream(FileMode.Create));
            }
        }


    }
}
