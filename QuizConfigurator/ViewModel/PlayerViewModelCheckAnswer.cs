
namespace QuizConfigurator.ViewModel;
public class PlayerViewModelCheckAnswer : BaseViewModel
{
    public bool IsVisibleCorrectIconAnswerZero { get; set; }
    public bool IsVisibleIncorrectIconAnswerZero { get; set; }
    public bool IsVisibleCorrectIconAnswerOne { get; set; }
    public bool IsVisibleIncorrectIconAnswerOne { get; set; }
    public bool IsVisibleCorrectIconAnswerTwo { get; set; }
    public bool IsVisibleIncorrectIconAnswerTwo { get; set; }
    public bool IsVisibleCorrectIconAnswerThree { get; set; }
    public bool IsVisibleIncorrectIconAnswerThree { get; set; }
    public async Task CheckAnswer(object selectedAnswer, PlayerViewModel playerViewModel)
    {
        if (selectedAnswer is string answer)
        {
            playerViewModel.IsAnswerCorrect = answer == playerViewModel.CurrentQuestion.CorrectAnswer;
            int selectedIndex = playerViewModel.CurrentAnswerOptions.IndexOf(answer);
            int correctIndex = playerViewModel.CurrentAnswerOptions.IndexOf(playerViewModel.CurrentQuestion.CorrectAnswer);

            HideAnswerIcons();

            DisplayCorrectAnswerIcon(correctIndex, playerViewModel);
            DisplaySelectedIncorrectAnswerIcon(selectedIndex, playerViewModel);

            await playerViewModel.ExecuteVoice(playerViewModel.IsAnswerCorrect ? "Correct" : "Wrong answer");

            if (playerViewModel.IsAnswerCorrect)
            {
                playerViewModel.AmountOfCorrectAnswers++;
            }

            UpdateIconVisibility(playerViewModel);

            await Task.Delay(1000);

            HideAnswerIcons();

            UpdateIconVisibility(playerViewModel);
        }
    }
    private void UpdateIconVisibility(PlayerViewModel playerViewModel)
    {
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerZero));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerZero));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerOne));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerOne));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerTwo));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerTwo));
        OnPropertyChanged(nameof(IsVisibleCorrectIconAnswerThree));
        OnPropertyChanged(nameof(IsVisibleIncorrectIconAnswerThree));
    }

    private void DisplayCorrectAnswerIcon(int correctIndex, PlayerViewModel playerViewModel)
    {
        switch (correctIndex)
        {
            case 0:
                IsVisibleCorrectIconAnswerZero = true;
                break;
            case 1:
                IsVisibleCorrectIconAnswerOne = true;
                break;
            case 2:
                IsVisibleCorrectIconAnswerTwo = true;
                break;
            case 3:
                IsVisibleCorrectIconAnswerThree = true;
                break;
        }
    }
    private void DisplaySelectedIncorrectAnswerIcon(int selectedIndex, PlayerViewModel playerViewModel)
    {
        if (!playerViewModel.IsAnswerCorrect)
        {
            switch (selectedIndex)
            {
                case 0:
                    IsVisibleIncorrectIconAnswerZero = true;
                    break;
                case 1:
                    IsVisibleIncorrectIconAnswerOne = true;
                    break;
                case 2:
                    IsVisibleIncorrectIconAnswerTwo = true;
                    break;
                case 3:
                    IsVisibleIncorrectIconAnswerThree = true;
                    break;
            }
        }
    }
    public void HideAnswerIcons()
    {
        IsVisibleCorrectIconAnswerZero = false;
        IsVisibleIncorrectIconAnswerZero = false;
        IsVisibleCorrectIconAnswerOne = false;
        IsVisibleIncorrectIconAnswerOne = false;
        IsVisibleCorrectIconAnswerTwo = false;
        IsVisibleIncorrectIconAnswerTwo = false;
        IsVisibleCorrectIconAnswerThree = false;
        IsVisibleIncorrectIconAnswerThree = false;
    }
}
