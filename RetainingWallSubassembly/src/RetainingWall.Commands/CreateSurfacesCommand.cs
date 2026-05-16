using System;
using System.Linq;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace RetainingWall.Commands
{
    public class CreateSurfacesCommand
    {
        private const string TopSurfaceName = "RW_Top_Surface";
        private const string EarthworkSurfaceName = "RW_Earthwork_Surface";
        private const string StructureSurfaceName = "RW_Structure_Reference";

        [CommandMethod("RW_CREATE_SURFACES")]
        public void CreateSurfaces()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            Editor ed = doc.Editor;
            Database db = doc.Database;
            CivilDocument civilDoc = CivilApplication.ActiveDocument;

            if (civilDoc == null)
            {
                ed.WriteMessage("\nError: No active Civil 3D document found.");
                return;
            }

            PromptEntityOptions options = new PromptEntityOptions("\nSelect a corridor to create retaining wall surfaces: ");
            options.SetRejectMessage("\nSelected object is not a corridor.");
            options.AddAllowedClass(typeof(Corridor), true);

            PromptEntityResult result = ed.GetEntity(options);
            if (result.Status != PromptStatus.OK) return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    Corridor corridor = tr.GetObject(result.ObjectId, OpenMode.ForWrite) as Corridor;
                    if (corridor == null)
                    {
                        ed.WriteMessage("\nError: Failed to open corridor.");
                        return;
                    }

                    ed.WriteMessage($"\nProcessing corridor: {corridor.Name}");

                    // 1. RW_Top_Surface
                    EnsureCorridorSurface(corridor, TopSurfaceName, new[] { "RW_Top" }, OverhangCorrectionType.TopLinks);

                    // 2. RW_Earthwork_Surface
                    EnsureCorridorSurface(corridor, EarthworkSurfaceName, new[] { "RW_Datum", "RW_Backfill", "RW_Excavation" }, OverhangCorrectionType.BottomLinks);

                    // 3. RW_Structure_Reference
                    EnsureCorridorSurface(corridor, StructureSurfaceName, new[] { "RW_PCC", "RW_Wall", "RW_Footing", "RW_Filter" }, OverhangCorrectionType.None);

                    corridor.Rebuild();
                    tr.Commit();
                    ed.WriteMessage("\nRetaining wall surfaces created/updated successfully.");
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage($"\nError: {ex.Message}");
                    tr.Abort();
                }
            }
        }

        private void EnsureCorridorSurface(Corridor corridor, string name, string[] linkCodes, OverhangCorrectionType overhangType)
        {
            CorridorSurface? surface = null;
            foreach (CorridorSurface s in corridor.CorridorSurfaces)
            {
                if (string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    surface = s;
                    break;
                }
            }

            if (surface == null)
            {
                surface = corridor.CorridorSurfaces.Add(name);
            }

            foreach (var code in linkCodes)
            {
                bool exists = false;
                string[] existingCodes = surface.LinkCodes();
                foreach (string existingCode in existingCodes)
                {
                    if (string.Equals(existingCode, code, StringComparison.OrdinalIgnoreCase))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    try
                    {
                        surface.AddLinkCode(code, true);
                    }
                    catch { /* Code might not exist in corridor yet */ }
                }
            }

            surface.OverhangCorrection = overhangType;

            // Add boundary if it doesn't exist
            if (surface.Boundaries.Count == 0)
            {
                try
                {
                    surface.Boundaries.AddCorridorExtentsBoundary("Corridor Extents");
                }
                catch { /* Might fail if no regions exist */ }
            }
        }
    }
}
