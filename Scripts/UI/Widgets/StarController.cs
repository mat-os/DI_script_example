using System.Collections.Generic;
using DG.Tweening;

public class StarController
{
    private readonly IStarUI[] _stars;         

    private readonly List<bool> _activatedStars = new();

    public StarController(IStarUI[] stars)
    {
        _stars = stars;

        for (int i = 0; i < _stars.Length; i++)
        {
            _activatedStars.Add(false);
            SetStarInactiveView(i);
        }
    }

    public void SetStarCount(int count)
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].SetActive(i < count);
            _stars[i].Deactivate();
            _activatedStars[i] = false;
        }
    }

    public void TryActivateStar(int index)
    {
        if (index >= 0 && index < _activatedStars.Count && !_activatedStars[index])
        {
            _activatedStars[index] = true;
            _stars[index].PlayActivateAnimation();
        }
    }
    
    private void SetStarInactiveView(int index)
    {
        if (index < 0 || index >= _stars.Length) 
            return;
        _stars[index].Deactivate();
    }
}