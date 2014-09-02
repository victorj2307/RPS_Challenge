<%@ Page Title="Tournament creation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RPS_Challenge._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>&nbsp;</h2>
            </hgroup>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>Follow these instructions:</h3>
    <ol class="round">
        <li class="one">
            <h5>Upload tournament</h5>
            Select the file that contains the definition of the tournament that you want to create.
            <br />
            <asp:FileUpload ID="btnTournamentFileUpload" runat="server" />
            <asp:RequiredFieldValidator ID="TournamentFileRequired" runat="server" ControlToValidate="btnTournamentFileUpload" Display="Dynamic" ErrorMessage="File is required" CssClass="message-error"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="TournamentFileExtensionValidator" runat="server" ErrorMessage="Only .txt file is allowed" ValidationExpression="^.+(.txt|.TXT)$" ControlToValidate="btnTournamentFileUpload" Display="Dynamic" CssClass="message-error"></asp:RegularExpressionValidator>
        </li>
        <li class="two">
            <h5>Play</h5>
            Press the "Play" button to begin the tournament and get the Winner.
            <br />
            <asp:Button ID="btnPlay" runat="server" Text="Play" CausesValidation="true" OnClick="btnPlay_Click" OnClientClick="javascript:hideResult();" />
            <asp:Label ID="lblResult" runat="server" CssClass="message-success"></asp:Label>
        </li>
        <li class="three">
            <h5>Scoreboard</h5>
            Press the "See scores" button to see the scoreboard of the players who joined the tournaments.
            <br />
            <asp:Button ID="btnSeeScores" runat="server" Text="See scores" CausesValidation="false" OnClick="btnSeeScores_Click" />
            <asp:GridView ID="scoreGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="PlayerName">
                <Columns>
                    <asp:BoundField DataField="PlayerName" HeaderText="Player Name" ReadOnly="True" SortExpression="PlayerName" HeaderStyle-Width="200px" />
                    <asp:BoundField DataField="Points" HeaderText="Points" SortExpression="Points" HeaderStyle-Width="50px" />
                </Columns>
            </asp:GridView>
        </li>
        <li class="four">
            <h5>Reset scoreboard</h5>
            Press the "Reset" button to reset the entire scoreboard.
            <br />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CausesValidation="false" OnClick="btnReset_Click" />
        </li>
    </ol>

    <h3>Tournament example files to download:</h3>
    <ol class="round">
        <li>
            <h4>
                <asp:LinkButton ID="fileDown1" runat="server" CausesValidation="false" OnClick="fileDown1_Click" Text="tournament4.txt"></asp:LinkButton></h4>
            File with the definition of a tournament of 4 players.
        </li>
        <li>
            <h4>
                <asp:LinkButton ID="fileDown2" runat="server" CausesValidation="false" OnClick="fileDown2_Click" Text="tournament12.txt"></asp:LinkButton></h4>
            File with the definition of a tournament of 12 players.
        </li>
        <li>
            <h4>
                <asp:LinkButton ID="fileDown3" runat="server" CausesValidation="false" OnClick="fileDown3_Click" Text="tournament16.txt"></asp:LinkButton></h4>
            File with the definition of a tournament of 16 players.
        </li>
        <li>
            <h4>
                <asp:LinkButton ID="fileDown4" runat="server" CausesValidation="false" OnClick="fileDown4_Click" Text="tournamentErrorStrategy.txt"></asp:LinkButton></h4>
            File with the definition of a tournament with invalid strategy.
        </li>
        <li>
            <h4>
                <asp:LinkButton ID="fileDown5" runat="server" CausesValidation="false" OnClick="fileDown5_Click" Text="tournamentError.txt"></asp:LinkButton></h4>
            File with the definition of a tournament with errors.
        </li>
    </ol>

    <script type="text/javascript">
        function hideResult() {
            document.getElementById("<%= this.lblResult.ClientID %>").style.visibility = 'hidden';
        }

    </script>
</asp:Content>


