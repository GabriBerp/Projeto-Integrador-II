using UnityEngine;

public class PoloComercial : PoloBase
{
    [Header("Comercial Info")] 
    private int maxHandWorkCapacity;
    [SerializeField] private int _baseMaxHandWorkCapacity;
    [SerializeField] private float _handWorkPrice;

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
            ProduceMoneyPerCycle();
            _cycletime = 0f;
        }  
    }

    public override void ProduceMoneyPerCycle()
    {
        int handWorkRemain = GameManager.Instance.handwork - maxHandWorkCapacity;
        float handWorkConsumed = maxHandWorkCapacity;
        if (handWorkRemain < 0)
        {
            /*
            Codigo para garantir que a quantidade coletada de lixo não vai ser maior que a quantidade atual de mão de obra.
            */
            handWorkConsumed = maxHandWorkCapacity - (handWorkRemain * -1f);
        }
        moneyPerTime = handWorkConsumed * _handWorkPrice;

        base.ProduceMoneyPerCycle();
        GameManager.Instance.handwork -= (int)handWorkConsumed; 
        
    }

    public override void OnClick()
    {
        base.OnClick();

        if (_cycletime < cycleMaxTime)
        {
            _cycletime += timeReductionPerClick;
        }else
        {
            ProduceMoneyPerCycle();
            _cycletime = 0f;
        } 
        // ShowPoloInfo();
        // Debug.Log(gameObject.name + ": apertou");
    }
}
