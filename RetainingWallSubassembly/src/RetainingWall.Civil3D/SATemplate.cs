using System;
using Autodesk.Civil.Runtime;
using Autodesk.Civil.ApplicationServices;

namespace RetainingWall.Civil3D
{
    public abstract class SATemplate
    {
        public void GetLogicalNames()
        {
            try
            {
                GetLogicalNamesImplement(CivilApplication.ActiveDocument.CorridorState);
            }
            catch { }
        }

        public void GetInputParameters()
        {
            try
            {
                GetInputParametersImplement(CivilApplication.ActiveDocument.CorridorState);
            }
            catch { }
        }

        public void GetOutputParameters()
        {
            try
            {
                GetOutputParametersImplement(CivilApplication.ActiveDocument.CorridorState);
            }
            catch { }
        }

        public void Draw()
        {
            try
            {
                CorridorState? state = null;
                try { state = CivilApplication.ActiveDocument.CorridorState; } catch { }
                if (state == null) state = CurrentCorridorState;

                if (state != null)
                    DrawImplement(state);
            }
            catch { }
        }

        protected virtual void GetLogicalNamesImplement(CorridorState corridorState) { }
        protected virtual void GetInputParametersImplement(CorridorState corridorState) { }
        protected virtual void GetOutputParametersImplement(CorridorState corridorState) { }
        protected abstract void DrawImplement(CorridorState corridorState);

        public CorridorState? CurrentCorridorState { get; set; }
    }
}
