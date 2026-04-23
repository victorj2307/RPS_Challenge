<%@ Page Title="Tournament creation" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RPS_Challenge._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="game-shell">
        <section class="game-hero">
            <div class="game-hero-title">Rock Paper Scissors Arena</div>
            <p class="game-hero-subtitle">Upload a bracket, launch the tournament, and watch the leaderboard evolve.</p>
        </section>

        <section class="mission-control">
            <div class="module-header">
                <h3>Mission Control</h3>
                <span id="playStatusChip" class="status-chip status-ready">Ready to play</span>
            </div>
            <ol class="round step-list step-grid">
                <li class="one control-card">
                    <h5>Step 1: Upload tournament file</h5>
                    <p>Select a <code>.txt</code> or <code>.json</code> tournament definition file.</p>
                    <asp:FileUpload ID="btnTournamentFileUpload" runat="server" />
                    <asp:RequiredFieldValidator ID="TournamentFileRequired" runat="server" ControlToValidate="btnTournamentFileUpload" Display="Dynamic" ErrorMessage="File is required" CssClass="message-error"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="TournamentFileExtensionValidator" runat="server" ErrorMessage="Only .txt or .json files are allowed" ValidationExpression="^.+(\.txt|\.TXT|\.json|\.JSON)$" ControlToValidate="btnTournamentFileUpload" Display="Dynamic" CssClass="message-error"></asp:RegularExpressionValidator>
                </li>
                <li class="two control-card">
                    <h5>Step 2: Play tournament</h5>
                    <p>Press <strong>Play</strong> to process the tournament and reveal the champion.</p>
                    <asp:Button ID="btnPlay" runat="server" Text="Play tournament" CssClass="play-button" CausesValidation="true" OnClick="btnPlay_Click" OnClientClick="return onActionButtonClick(this, 'Playing...');" />
                    <asp:Panel ID="pnlResult" runat="server" CssClass="result-panel" Visible="false">
                        <asp:Label ID="lblResult" runat="server" CssClass="result-message"></asp:Label>
                    </asp:Panel>
                </li>
            </ol>
        </section>

        <div class="tournament-dashboard">
        <section id="roundResultsSection" runat="server" class="round-results-section" visible="false">
            <asp:Panel ID="pnlRoundResults" runat="server" Visible="false">
                <div class="module-header">
                    <h3>Round timeline</h3>
                    <span class="module-subtitle">Detailed results for each match and advancement</span>
                </div>
                <asp:Repeater ID="roundResultsRepeater" runat="server" OnItemDataBound="roundResultsRepeater_ItemDataBound">
                    <ItemTemplate>
                        <div class="round-result-card">
                            <h4>Round <%# Eval("RoundNumber") %></h4>
                            <div class="round-summary">Matches: <%# Eval("MatchCount") %></div>
                            <asp:Repeater ID="matchResultsRepeater" runat="server">
                                <ItemTemplate>
                                    <div class="round-match-line">
                                        <asp:PlaceHolder runat="server" Visible='<%# !(bool)Eval("IsBye") %>'>
                                            <div class="round-match-versus">
                                                <div class='round-player <%# (bool)Eval("LeftPlayerWon") ? "round-player-winner" : "round-player-loser" %>'>
                                                    <span class="round-player-name"><%# Eval("LeftPlayerName") %></span>
                                                    <span class="round-player-strategy">
                                                        <span class="round-strategy-label"><%# Eval("LeftPlayerStrategy") %></span>
                                                        <span class="round-strategy-icon"><%# GetStrategyIcon(Eval("LeftPlayerStrategy")) %></span>
                                                    </span>
                                                </div>
                                                <div class="round-versus-badge">VS</div>
                                                <div class='round-player <%# (bool)Eval("RightPlayerWon") ? "round-player-winner" : "round-player-loser" %>'>
                                                    <span class="round-player-name"><%# Eval("RightPlayerName") %></span>
                                                    <span class="round-player-strategy">
                                                        <span class="round-strategy-label"><%# Eval("RightPlayerStrategy") %></span>
                                                        <span class="round-strategy-icon"><%# GetStrategyIcon(Eval("RightPlayerStrategy")) %></span>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="round-match-loser-line">
                                                Winner: <span class="round-match-winner"><%# Eval("WinnerName") %></span>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("IsBye") %>'>
                                            <div class="round-match-bye-line">No opponent this round — advanced to the next round</div>
                                        </asp:PlaceHolder>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <asp:Panel ID="pnlChampionSummary" runat="server" CssClass="champion-summary-section" Visible="false">
                <div class="round-champion-line">
                    <div class="round-champion-left">
                        <div class="round-champion-title">Champion Crowned</div>
                        <div class="round-champion-strategy">
                            Strategy: <asp:Label ID="lblChampionStrategyIcon" runat="server" /> <asp:Label ID="lblChampionStrategy" runat="server" />
                        </div>
                    </div>
                    <div class="round-champion-center">
                        <span class="round-champion-name"><asp:Label ID="lblChampionName" runat="server" /></span>
                    </div>
                    <div class="round-champion-right">
                        <span class="round-champion-trophy" aria-hidden="true">🏆</span>
                    </div>
                </div>
            </asp:Panel>
        </section>

        <section class="scoreboard-section">
            <div class="module-header">
                <h3>Leaderboard</h3>
                <span class="module-subtitle">Top tournament players ranked by total points</span>
            </div>
            <asp:Button ID="btnReset" runat="server" Text="Reset scoreboard" CausesValidation="false" OnClick="btnReset_Click" OnClientClick="return confirmResetAndSubmit(this);" />
            <asp:GridView ID="scoreGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="PlayerName" EmptyDataText="No scores available yet. Play a tournament to generate scores." OnRowDataBound="scoreGrid_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Rank" HeaderText="#" HeaderStyle-Width="40px" ItemStyle-CssClass="score-rank" />
                    <asp:BoundField DataField="PlayerName" HeaderText="Player Name" ReadOnly="True" SortExpression="PlayerName" HeaderStyle-Width="200px" />
                    <asp:BoundField DataField="Points" HeaderText="Points" SortExpression="Points" HeaderStyle-Width="70px" ItemStyle-CssClass="score-points" />
                </Columns>
            </asp:GridView>
        </section>
        </div>
    </div>

    <script type="text/javascript">
        var isSubmittingForm = false;

        function markFormSubmitting() {
            isSubmittingForm = true;
            return true;
        }

        function onActionButtonClick(buttonElement, loadingText) {
            if (buttonElement.disabled) {
                return false;
            }
            var statusChip = document.getElementById("playStatusChip");
            if (statusChip) {
                statusChip.className = "status-chip status-processing";
                statusChip.textContent = "Processing tournament";
            }

            var originalText = buttonElement.getAttribute("data-original-text");
            if (!originalText) {
                originalText = buttonElement.value;
                buttonElement.setAttribute("data-original-text", originalText);
            }

            buttonElement.value = loadingText;

            window.setTimeout(function () {
                if (!isSubmittingForm) {
                    buttonElement.disabled = false;
                    buttonElement.value = originalText;
                    if (statusChip) {
                        statusChip.className = "status-chip status-ready";
                        statusChip.textContent = "Ready to play";
                    }
                }
            }, 1200);
            return true;
        }

        function confirmResetAndSubmit(buttonElement) {
            if (!confirm("Are you sure you want to reset the scoreboard? This action cannot be undone.")) {
                return false;
            }

            return onActionButtonClick(buttonElement, "Resetting...");
        }
    </script>
</asp:Content>


