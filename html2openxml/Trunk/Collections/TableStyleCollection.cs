﻿/* Copyright (C) Olivier Nizet http://html2openxml.codeplex.com - All Rights Reserved
 * 
 * This source is subject to the Microsoft Permissive License.
 * Please see the License.txt file for more information.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 */
using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace NotesFor.HtmlToOpenXml
{
	using TagsAtSameLevel = System.ArraySegment<DocumentFormat.OpenXml.OpenXmlElement>;


	sealed class TableStyleCollection : OpenXmlStyleCollectionBase
	{
		private ParagraphStyleCollection paragraphStyle;
		private HtmlDocumentStyle documentStyle;
		private static GetSequenceNumberHandler getTagOrderHandler;


		internal TableStyleCollection(HtmlDocumentStyle documentStyle)
		{
			this.documentStyle = documentStyle;
			paragraphStyle = new ParagraphStyleCollection(documentStyle);
		}

		internal override void Reset()
		{
			paragraphStyle.Reset();
			base.Reset();
		}

		//____________________________________________________________________
		//

		/// <summary>
		/// Apply all the current Html tag to the specified table cell.
		/// </summary>
		public override void ApplyTags(OpenXmlCompositeElement tableCell)
		{
			if (tags.Count > 0)
			{
				TableCellProperties properties = tableCell.GetFirstChild<TableCellProperties>();
				if (properties == null) tableCell.PrependChild<TableCellProperties>(properties = new TableCellProperties());

				var en = tags.GetEnumerator();
				while (en.MoveNext())
				{
					TagsAtSameLevel tagsOfSameLevel = en.Current.Value.Peek();
					foreach (OpenXmlElement tag in tagsOfSameLevel.Array)
						SetProperties(properties, tag.CloneNode(true));
				}
			}

			// Apply some style attributes on the unique Paragraph tag contained inside a table cell.
			Paragraph p = tableCell.GetFirstChild<Paragraph>();
			paragraphStyle.ApplyTags(p);
		}

		public void BeginTagForParagraph(string name, params OpenXmlElement[] elements)
		{
			paragraphStyle.BeginTag(name, elements);
		}

		public override void EndTag(string name)
		{
			paragraphStyle.EndTag(name);
			base.EndTag(name);
		}

		#region ProcessCommonAttributes

		/// <summary>
		/// Move inside the current tag related to table (td, thead, tr, ...) and converts some common
		/// attributes to their OpenXml equivalence.
		/// </summary>
		/// <param name="en">The Html enumerator positionned on a <i>table (or related)</i> tag.</param>
		/// <param name="runStyleAttributes">The collection of attributes where to store new discovered attributes.</param>
		public void ProcessCommonAttributes(HtmlEnumerator en, IList<OpenXmlElement> runStyleAttributes)
		{
			List<OpenXmlElement> containerStyleAttributes = new List<OpenXmlElement>();

			var colorValue = en.StyleAttributes.GetAsColor("background-color");

            // "background-color" is also handled by RunStyleCollection which duplicate this attribute (bug #13212). Let's ignore it
            if (!colorValue.IsEmpty && en.CurrentTag.Equals("<td>", StringComparison.InvariantCultureIgnoreCase)) colorValue = System.Drawing.Color.Empty;
			if (colorValue.IsEmpty) colorValue = en.Attributes.GetAsColor("bgcolor");
            if (!colorValue.IsEmpty)
			{
				containerStyleAttributes.Add(
					new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = colorValue.ToHexString() });
			}

			var htmlAlign = en.StyleAttributes["vertical-align"];
			if (htmlAlign == null) htmlAlign = en.Attributes["valign"];
			if (htmlAlign != null)
			{
				TableVerticalAlignmentValues? valign = ConverterUtility.FormatVAlign(htmlAlign);
				if (valign.HasValue)
					containerStyleAttributes.Add(new TableCellVerticalAlignment() { Val = valign });
			}

			htmlAlign = en.StyleAttributes["text-align"];
			if (htmlAlign == null) htmlAlign = en.Attributes["align"];
			if (htmlAlign != null)
			{
				JustificationValues? halign = ConverterUtility.FormatParagraphAlign(htmlAlign);
				if (halign.HasValue)
					this.BeginTagForParagraph(en.CurrentTag, new KeepNext(), new Justification { Val = halign });
			}

			// implemented by ddforge
			String[] classes = en.Attributes.GetAsClass();
			if (classes != null)
			{
				for (int i = 0; i < classes.Length; i++)
				{
					string className = documentStyle.GetStyle(classes[i], StyleValues.Table, ignoreCase: true);
					if (className != null) // only one Style can be applied in OpenXml and dealing with inheritance is out of scope
					{
						containerStyleAttributes.Add(new RunStyle() { Val = className });
						break;
					}
				}
			}

			this.BeginTag(en.CurrentTag, containerStyleAttributes);

			// Process general run styles
			documentStyle.Runs.ProcessCommonAttributes(en, runStyleAttributes);
		}

		#endregion

		#region GetTagOrder

		protected override int GetTagOrder(OpenXmlElement element)
		{
			// I don't want to hard-code the sequence number of the child elements of a RunProperties.
			// I prefer relying on the OpenXml API and use a bit Reflection.
			if (getTagOrderHandler == null)
			{
				var mi = typeof(OpenXmlCompositeElement)
					.GetMethod("GetSequenceNumber", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                // We use a dummy new TableProperties instance
                // Create a delegate to speed up the invocation to the GetSequenceNumber method
                getTagOrderHandler = (GetSequenceNumberHandler)
					Delegate.CreateDelegate(typeof(GetSequenceNumberHandler), new TableProperties(), mi, true);
			}

			return (int) getTagOrderHandler.DynamicInvoke(element);
		}

		#endregion
	}
}