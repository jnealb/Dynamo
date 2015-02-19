using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using ProtoCore.AssociativeGraph;
using ProtoCore.AssociativeEngine;
using ProtoCore.AST;
using ProtoCore.AST.AssociativeAST;
using ProtoCore.BuildData;
using ProtoCore.CodeModel;
using ProtoCore.DebugServices;
using ProtoCore.DSASM;
using ProtoCore.Lang;
using ProtoCore.Lang.Replication;
using ProtoCore.Runtime;
using ProtoCore.Utils;
using ProtoFFI;

using StackFrame = ProtoCore.DSASM.StackFrame;

namespace ProtoCore
{
    public class InterpreterProperties
    {
        public GraphNode executingGraphNode { get; set; }
        public List<GraphNode> nodeIterations { get; set; }

        public List<StackValue> functionCallArguments { get; set; }
        public List<StackValue> functionCallDotCallDimensions { get; set; }

        public UpdateStatus updateStatus { get; set; }

        public InterpreterProperties()
        {
            Reset();
        }

        public InterpreterProperties(InterpreterProperties rhs)
        {
            executingGraphNode = rhs.executingGraphNode;
            nodeIterations = rhs.nodeIterations;
            functionCallArguments = rhs.functionCallArguments;
            functionCallDotCallDimensions = rhs.functionCallDotCallDimensions;
            updateStatus = rhs.updateStatus;
        }

        public void Reset()
        {
            executingGraphNode = null;
            nodeIterations = new List<GraphNode>();
            functionCallArguments = new List<StackValue>();
            functionCallDotCallDimensions = new List<StackValue>();
            updateStatus = UpdateStatus.kNormalUpdate;
        }
    }

    /// <summary>
    /// RuntimeCore is an object that is instantiated once across the lifecycle of the runtime
    /// This is the entry point of the runtime VM and its input is a DS Executable format. 
    /// There will only be one instance of RuntimeCore regardless of how many times instances of a DSASM.Executive (runtime VM) is instantiated.
    /// Its properties will be persistent and accessible across all instances of a DSASM.Executive
    /// </summary>
    public class RuntimeCore
    {
        public RuntimeCore()
        {
            InterpreterProps = new Stack<InterpreterProperties>();
            ReplicationGuides = new List<List<ReplicationGuide>>();
        }

        public void SetProperties(Options runtimeOptions, Executable executable, DebugProperties debugProps = null, ProtoCore.Runtime.Context context = null)
        {
            this.Context = context;
            this.DSExecutable = executable;
            this.Options = runtimeOptions;
            this.DebugProps = debugProps;
        }


        public Executable DSExecutable { get; private set; }
        public Options Options { get; private set; }
        public RuntimeStatus RuntimeStatus { get; set; }
        public Stack<InterpreterProperties> InterpreterProps { get; set; }

        public RuntimeMemory RuntimeMemory { get; set; }
        public ProtoCore.Runtime.Context Context { get; set; }

        /// <summary>
        /// RuntimeExpressionUID is used by the associative engine at runtime to determine the current expression ID being executed
        /// </summary>
        public int RuntimeExpressionUID = 0;

        // Cached replication guides for the current call. 
        // TODO Jun: Store this in the dynamic table node
        public List<List<ReplicationGuide>> ReplicationGuides;

#region DEBUGGER_PROPERTIES
        public DebugProperties DebugProps { get; set; }
        public List<Instruction> Breakpoints { get; set; }
#endregion 

        
        public bool IsEvalutingPropertyChanged()
        {
            foreach (var prop in InterpreterProps)
            {
                if (prop.updateStatus == UpdateStatus.kPropertyChangedUpdate)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
