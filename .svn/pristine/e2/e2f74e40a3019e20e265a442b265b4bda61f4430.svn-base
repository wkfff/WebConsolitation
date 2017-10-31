using System;
using System.Collections.Generic;
using System.Text;

using Itenso.Rtf;
using Itenso.Rtf.Converter.Image;
using Itenso.Rtf.Parser;
using System.IO;
using Itenso.Rtf.Support;
using Itenso.Rtf.Interpreter;
using Itenso.Rtf.Converter.Html;
using System.Diagnostics;


namespace Krista.FM.Client.Help
{
    /// <summary>
    /// 
    /// </summary>
    public class ConvertCDATASection
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ConvertSettings settings;
        /// <summary>
        /// 
        /// </summary>
        public ConvertCDATASection()
		{
			this.settings = new ConvertSettings();
		}

        /// <summary>
        /// Вход в функцию конвертации
        /// </summary>
        /// <param name="inputString"> Входная сторока</param>
        /// <returns> Конвертированная строка</returns>
		public String Execute(String inputString)
		{
			// program settings
			if ( ValidateProgramSettings() == false )
			{
                //
			}

			// parse rtf
			IRtfGroup rtfStructure = ParseRtf(inputString);
			if ( Environment.ExitCode != 0 )
			{
                //
			}

			// image handling
			RtfVisualImageAdapter imageAdapter = new RtfVisualImageAdapter(
				this.settings.ImageFileNamePattern,
				this.settings.ImageFormat );

			// interpret rtf
			IRtfDocument rtfDocument = InterpretRtf( rtfStructure, imageAdapter );
			if ( Environment.ExitCode != 0 )
			{
                //
			}

			// convert to hmtl
			string html = ConvertHmtl( rtfDocument, imageAdapter );
			if ( Environment.ExitCode != 0 )
			{
                //
			}

            return html;

		}

		// ----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		private bool ValidateProgramSettings()
		{
			if ( !this.settings.IsValid )
			{
				Environment.ExitCode = -1;
				return false;
			}

			return true;
		} // ValidateProgramSettings

		// ----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
		private IRtfGroup ParseRtf(String inputString)
		{
			IRtfGroup rtfStructure;
			RtfParserListenerFileLogger parserLogger = null;
			try
			{
				// logger
				if ( this.settings.LogParser )
				{
					string logFileName = this.settings.BuildDestinationFileName( 
						this.settings.LogDirectory,
						RtfParserListenerFileLogger.DefaultLogFileExtension );
					parserLogger = new RtfParserListenerFileLogger( logFileName );
				}

				// rtf parser
				// parse the rtf structure
				RtfParserListenerStructureBuilder structureBuilder = new RtfParserListenerStructureBuilder();
				RtfParser parser = new RtfParser( structureBuilder );
				parser.IgnoreContentAfterRootGroup = true; // support WordPad documents
				if ( parserLogger != null )
				{
					parser.AddParserListener( parserLogger );
				}
				parser.Parse( new RtfSource( inputString ) );
				rtfStructure = structureBuilder.StructureRoot;
			}
			catch ( Exception e )
			{
				if ( parserLogger != null )
				{
					parserLogger.Dispose();
				}

				//throw new Exception( "error while parsing rtf: " + e.Message );
				Environment.ExitCode = -2;
				return null;
			}

			return rtfStructure;
		} // ParseRtf

		// ----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rtfStructure"></param>
        /// <param name="imageAdapter"></param>
        /// <returns></returns>
		private IRtfDocument InterpretRtf( IRtfGroup rtfStructure, IRtfVisualImageAdapter imageAdapter )
		{
			IRtfDocument rtfDocument;
			RtfInterpreterListenerFileLogger interpreterLogger = null;
			try
			{
				// logger
				if ( this.settings.LogInterpreter )
				{
					string logFileName = this.settings.BuildDestinationFileName(
						this.settings.LogDirectory,
						RtfInterpreterListenerFileLogger.DefaultLogFileExtension );
					interpreterLogger = new RtfInterpreterListenerFileLogger( logFileName );
				}

				// image converter
				RtfImageConverter imageConverter = null;
				if ( this.settings.SaveImage )
				{
					RtfImageConvertSettings imageConvertSettings = new RtfImageConvertSettings( imageAdapter );
					imageConvertSettings.ImagesPath = this.settings.DestinationDirectory;
					if ( this.settings.ExtendedImageScale )
					{
						imageConvertSettings.ScaleExtension = 0.5f;
					}
					imageConverter = new RtfImageConverter( imageConvertSettings );
				}

				// rtf parser
				// interpret the rtf structure using the extractors
				rtfDocument = RtfInterpreterTool.BuildDoc( rtfStructure, interpreterLogger, imageConverter );

			}
			catch ( Exception e )
			{
				if ( interpreterLogger != null )
				{
					interpreterLogger.Dispose();
				}

				//throw new Exception( "error while interpreting rtf: " + e.Message );
				Environment.ExitCode = -4;
				return null;
			}

			return rtfDocument;
		} // InterpretRtf

		// ----------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rtfDocument"></param>
        /// <param name="imageAdapter"></param>
        /// <returns></returns>
		private string ConvertHmtl( IRtfDocument rtfDocument, IRtfVisualImageAdapter imageAdapter )
		{
			string html;

			try
			{
				RtfHtmlConvertSettings htmlConvertSettings = new RtfHtmlConvertSettings( imageAdapter );
				if ( this.settings.CharacterSet != null )
				{
					htmlConvertSettings.CharacterSet = this.settings.CharacterSet;
				}
				htmlConvertSettings.Title = this.settings.SourceFileNameWithoutExtension;
				htmlConvertSettings.ImagesPath = this.settings.ImagesPath;
				htmlConvertSettings.IsShowHiddenText = this.settings.ShowHiddenText;
				if ( this.settings.ConvertScope != RtfHtmlConvertScope.None )
				{
					htmlConvertSettings.ConvertScope = this.settings.ConvertScope;
				}
				if ( !string.IsNullOrEmpty( this.settings.StyleSheets ) )
				{
					string[] styleSheets = this.settings.StyleSheets.Split( ',' );
					htmlConvertSettings.StyleSheetLinks.AddRange( styleSheets );
				}
				htmlConvertSettings.ConvertVisualHyperlinks = this.settings.ConvertVisualHyperlinks;
				if ( !string.IsNullOrEmpty( this.settings.VisualHyperlinkPattern ) )
				{
					htmlConvertSettings.VisualHyperlinkPattern = this.settings.VisualHyperlinkPattern;
				}

				RtfHtmlConverter htmlConverter = new RtfHtmlConverter( rtfDocument, htmlConvertSettings );
				html = htmlConverter.Convert();
			}
			catch ( Exception e )
			{
				//throw new Exception( "error while converting to html: " + e.Message );
				Environment.ExitCode = -5;
				return null;
			}

			return html;
		} // ConvertHmtl
    }
}
