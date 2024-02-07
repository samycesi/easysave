using Easysave.Controller;
using Easysave.Logger;
using Easysave.View;

class Program
{
    static void Main(string[] args)
    {
        ConsoleView view = new ConsoleView();

        view.DisplayMenu(); // Démarrer l'interaction avec l'utilisateur
    }
}