using UnityEngine;

public class PoloResidencial : PoloBase
{
    [Header("Residencial Info")] 
    private int maxHandWorkCapacity;
    [SerializeField] private int _baseMaxHandWorkCapacity;

    public override void Awake() {
        base.Awake();
        maxHandWorkCapacity = _baseMaxHandWorkCapacity;
    }

   void FixedUpdate() {
        if (_cycletime < cycleMaxTime)
        {
            _cycletime += Time.deltaTime;
            // Debug.Log(_cycletime);
        }
        else
        {
            ProduceHandWorkPerCycle();
            _cycletime = 0f;
        }  
    }

    public override void ProduceHandWorkPerCycle()
    {
        moneyPerTime =  maxHandWorkCapacity; // reaproveitando variavel.
        base.ProduceHandWorkPerCycle();
    }

    public override void OnClick()
    {
        base.OnClick();

        if (_cycletime < cycleMaxTime)
        {
            _cycletime += timeReductionPerClick;
        }else
        {
            ProduceHandWorkPerCycle();
            _cycletime = 0f;
        } 
        // ShowPoloInfo();
        Debug.Log(gameObject.name + ": apertou");
    }
}
