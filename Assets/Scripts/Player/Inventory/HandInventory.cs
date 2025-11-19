using UnityEngine;
using UnityEngine.UI;

public enum Hand
{
    Left,
    Right
}

public class HandInventory : MonoBehaviour
{
    private HandSlot leftHandSlot;
    private HandSlot rightHandSlot;
    private HandSlot leftPocketSlot;
    private HandSlot rightPocketSlot;

    private bool isLeftHandActive = true;
    private bool isRightHandActive = true;

    [Header("Item in hands")]
    [SerializeField] private GameObject _leftInHandObject;
    [SerializeField] private GameObject _rightInHandObject;

    [Header("Pockets")]
    [SerializeField] private Image _leftPocket;
    [SerializeField] private Image _rightPocket;
    [SerializeField] private Sprite defaultSprite;

    void Awake()
    {
        leftHandSlot = new HandSlot();
        rightHandSlot = new HandSlot();
        leftPocketSlot = new HandSlot();
        rightPocketSlot = new HandSlot();
    }

    void Start()
    {
        _leftPocket.sprite = defaultSprite;
        _rightPocket.sprite = defaultSprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            SwapSlots(Hand.Left);

        if (Input.GetKeyDown(KeyCode.P))
            SwapSlots(Hand.Right);

        if (Input.GetKeyDown(KeyCode.I))
            SwapHands();
    }

    private void OnItemEquipped(Hand hand, ItemData item)
    {
        if (item.isTwoHanded)
        {
            if (hand == Hand.Left)
            {
                isRightHandActive = false;
            }
            else
            {
                isLeftHandActive = false;
            }
        }
        GameObject targetObject = (hand == Hand.Left) ? _leftInHandObject : _rightInHandObject;
        targetObject.GetComponent<MeshFilter>().mesh = item.mesh;
    }

    private void OnItemUnequipped(Hand hand, ItemData item)
    {
        if (item.isTwoHanded)
        {
            isLeftHandActive = true;
            isRightHandActive = true;
        }
        GameObject targetObject = (hand == Hand.Left) ? _leftInHandObject : _rightInHandObject;
        targetObject.GetComponent<MeshFilter>().mesh = null;
    }

    public bool TryEquip(ItemData item)
    {
        if (isRightHandActive && rightHandSlot.IsEmpty())
            return TryEquipToHand(item, Hand.Right);
        if (isLeftHandActive && leftHandSlot.IsEmpty())
            return TryEquipToHand(item, Hand.Left);
        return false;
    }

    public bool TryEquipToHand(ItemData item, Hand hand)
    {
        HandSlot targetSlot = (hand == Hand.Left) ? leftHandSlot : rightHandSlot;
        bool isHandActive = (hand == Hand.Left) ? isLeftHandActive : isRightHandActive;

        if (isHandActive && targetSlot.IsEmpty())
        {
            if (item.isTwoHanded)
            {
                if (leftHandSlot.IsEmpty() && rightHandSlot.IsEmpty() && isLeftHandActive && isRightHandActive)
                {
                    EquipToHand(targetSlot, item, hand);
                    return true;
                }
                else
                {
                    Debug.Log("Cannot equip two-handed item, both hands must be free.");
                    return false;
                }
            }
            else
            {
                EquipToHand(targetSlot, item, hand);
                return true;
            }
        }
        Debug.Log("Cannot equip item, hand is not empty or not active.");
        return false;
    }

    private void EquipToHand(HandSlot slot, ItemData item, Hand hand)
    {
        slot.Equip(item);
        OnItemEquipped(hand, item);
    }

    public void SwapSlots(Hand hand)
    {
        HandSlot handSlot = (hand == Hand.Left) ? leftHandSlot : rightHandSlot;
        HandSlot pocketSlot = (hand == Hand.Left) ? leftPocketSlot : rightPocketSlot;
        bool isHandActive = (hand == Hand.Left) ? isLeftHandActive : isRightHandActive;

        if (!isHandActive)
        {
            Debug.Log("Cannot swap, hand slot is not active.");
            return;
        }

        // Unequip from both slots
        ItemData unequippedHandItem = handSlot.Unequip();
        ItemData unequippedPocketItem = pocketSlot.Unequip();

        if (unequippedPocketItem != null && unequippedPocketItem.isTwoHanded)
        {
            if (!leftHandSlot.IsEmpty() || !rightHandSlot.IsEmpty())
            {
                Debug.Log("Cannot equip two-handed item from pocket, both hands must be free.");
                handSlot.Equip(unequippedHandItem);
                pocketSlot.Equip(unequippedPocketItem);
                return;
            }
        }

        // Re-equip to swapped slots
        if (unequippedHandItem != null)
        {
            OnItemUnequipped(hand, unequippedHandItem);
            pocketSlot.Equip(unequippedHandItem);
            OnItemPutToPocket(hand, unequippedHandItem);
        }

        if (unequippedPocketItem != null)
        {
            handSlot.Equip(unequippedPocketItem);
            if (unequippedHandItem == null)
                OnItemGetFromPocket(hand, unequippedPocketItem);
            OnItemEquipped(hand, unequippedPocketItem);
        }
    }

    public void SwapHands()
    {
        if (isLeftHandActive && isRightHandActive)
        {
            ItemData item_1 = UnequipFromHand(Hand.Left);
            ItemData item_2 = UnequipFromHand(Hand.Right);

            if (item_1 != null)
                EquipToHand(rightHandSlot, item_1, Hand.Right);
            if (item_2 != null)
                EquipToHand(leftHandSlot, item_2, Hand.Left);
        }
    }
    
    public ItemData UnequipFromHand(Hand hand)
    {
        HandSlot handSlot = (hand == Hand.Left) ? leftHandSlot : rightHandSlot;
        if (!handSlot.IsEmpty())
        {
            ItemData item = handSlot.Unequip();
            OnItemUnequipped(hand, item);
            return item;
        }
        return null;
    }

    private void OnItemPutToPocket(Hand pocket, ItemData item)
    {
        Image pocketImage = (pocket == Hand.Left) ? _leftPocket : _rightPocket;
        pocketImage.sprite = item.icon;
    }

    private void OnItemGetFromPocket(Hand pocket, ItemData item)
    {
        Image pocketImage = (pocket == Hand.Left) ? _leftPocket : _rightPocket;
        pocketImage.sprite = defaultSprite;
    }
}
