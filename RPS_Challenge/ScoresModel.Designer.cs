﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
namespace RPS_Challenge
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class ScoresEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new ScoresEntities object using the connection string found in the 'ScoresEntities' section of the application configuration file.
        /// </summary>
        public ScoresEntities() : base("name=ScoresEntities", "ScoresEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ScoresEntities object.
        /// </summary>
        public ScoresEntities(string connectionString) : base(connectionString, "ScoresEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new ScoresEntities object.
        /// </summary>
        public ScoresEntities(EntityConnection connection) : base(connection, "ScoresEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Score> Scores
        {
            get
            {
                if ((_Scores == null))
                {
                    _Scores = base.CreateObjectSet<Score>("Scores");
                }
                return _Scores;
            }
        }
        private ObjectSet<Score> _Scores;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Scores EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToScores(Score score)
        {
            base.AddObject("Scores", score);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="ScoresModel", Name="Score")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Score : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Score object.
        /// </summary>
        /// <param name="playerName">Initial value of the PlayerName property.</param>
        /// <param name="points">Initial value of the Points property.</param>
        public static Score CreateScore(global::System.String playerName, global::System.Int32 points)
        {
            Score score = new Score();
            score.PlayerName = playerName;
            score.Points = points;
            return score;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String PlayerName
        {
            get
            {
                return _PlayerName;
            }
            set
            {
                if (_PlayerName != value)
                {
                    OnPlayerNameChanging(value);
                    ReportPropertyChanging("PlayerName");
                    _PlayerName = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("PlayerName");
                    OnPlayerNameChanged();
                }
            }
        }
        private global::System.String _PlayerName;
        partial void OnPlayerNameChanging(global::System.String value);
        partial void OnPlayerNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Points
        {
            get
            {
                return _Points;
            }
            set
            {
                OnPointsChanging(value);
                ReportPropertyChanging("Points");
                _Points = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Points");
                OnPointsChanged();
            }
        }
        private global::System.Int32 _Points;
        partial void OnPointsChanging(global::System.Int32 value);
        partial void OnPointsChanged();

        #endregion

    
    }

    #endregion

    
}
