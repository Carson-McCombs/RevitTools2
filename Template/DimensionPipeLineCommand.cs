﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace CarsonsAddins
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class DimensionPipeLineCommand : IExternalCommand, ISettingsComponent
    {
        public const bool IsWIP = true;
        public PushButtonData RegisterButton(Assembly assembly)
        {
            PushButtonData pushButtonData = new PushButtonData("DimensionPipeLineCommand_WIP", "Dimensions Pipe Line (WIP)", assembly.Location, "CarsonsAddins.DimensionPipeLineCommand");
            pushButtonData.ToolTip = "Gets the dimensions of all elements in pipe line.";
            return pushButtonData;
        }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }
        public Result Execute(UIApplication uiapp)
        {
            int flag = 0;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            if (doc.IsFamilyDocument)
            {
                TaskDialog.Show("Question Mark Dimensions Command", "Command should not be used within a family document.");
                return Result.Failed;
            }
            Transaction transaction = new Transaction(doc);
            transaction.Start("DimensionPipeLineCommand");
            try
            {
                //Reference elementReference = uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter_PipingElements(true, true, true, true), "Please select a Pipe, Flange, Bend, or Accessory");
                //Element elem = doc.GetElement(elementReference);
                //if (elem == null)
                //{
                //    transaction.RollBack();
                //    return Result.Cancelled;
                //}
                //string s = "";
                //XYZ[] points = Util.GetDimensionPoints(elem);
                //if (points == null)
                //{
                //    TaskDialog.Show("Get Dimension Points from Element", "Element Dimension Points are null");
                //    transaction.RollBack();
                //    return Result.Failed;
                //}
                //foreach (XYZ point in points)
                //{
                //    s = s + point.ToString() + "\n";
                //}
                //TaskDialog.Show("Get Dimension Points from Element",s);
                
                Reference pipeReference = uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter_Pipe(), "Please select a Pipe.");
                Pipe pipe = doc.GetElement(pipeReference.ElementId) as Pipe;
                flag++;
                if (pipe == null)
                {
                    transaction.RollBack();
                    TaskDialog.Show("DPL Error", "Pipe is null");
                    return Result.Cancelled;
                }
                PipeLine pipeLine = new PipeLine();
                pipeLine.GetPipeLine(uidoc, pipe, new SelectionFilter_PipingElements(true, true, false, true));
                flag++;
                pipeLine.CreateDimensionLinesFromReferences(doc, 4);
                flag++;
                transaction.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("DPL Error " + flag, ex.Message);
               
                transaction.RollBack();
                return Result.Failed;
            }
        }

        
    }
}
