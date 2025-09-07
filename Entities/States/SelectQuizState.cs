namespace repetitorbot.Entities.States;

internal class SelectQuizState : State
{
    public int? MessageId { get; set; }
    public int ItemsPerPage { get; set; }
    public int PagesCount { get; set; }
    public int CurrentPage { get; set; }
}
