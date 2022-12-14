using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static XP.TableModel.HeaderColumnCell;

namespace XP.TableModel
{
    public delegate void _CellDataChangeDelegate(HeaderCellBase cell, HeaderCellData columnCellData);
    /// <summary>
    /// �Ƿ��ڱ߽���ί��
    /// </summary>
    /// <param name="cell">��Ԫ��</param>
    /// <param name="isInsideBoundary">�Ƿ��ڱ߽���</param>
    public delegate void _IsInsideBoundaryChangeDelegate(HeaderCellBase cell, bool isInsideBoundary);
    /// <summary>
    /// ��ͷ��Ԫ�����
    /// </summary>
    public abstract  class HeaderCellBase : Toggle
    {
        [Header("�����������仯ʱ����")]
        public InputField.OnChangeEvent _OnCellNameChanged;
        /// <summary>
        /// �����ݷ����仯ʱ����
        /// </summary>
        public event _CellDataChangeDelegate _OnCellDataChangeEvent;
        /// <summary>
        /// ��ק��ť
        /// </summary>
        public HeaderDragButton _DragButton;

        HeaderCellData cellData;
        /// <summary>
        /// ��Ԫ������
        /// </summary>
        public HeaderCellData _CellData
        {
            get => cellData; set
            {
                if (cellData == value) return;
                if (cellData != null)
                {
                    cellData.PropertyChanged -= _CellData_PropertyChanged;
                }
                cellData = value;
                _OnCellDataChangeEvent?.Invoke(this, value);
                if (value == null) return;

                value.PropertyChanged -= _CellData_PropertyChanged;
                value.PropertyChanged += _CellData_PropertyChanged;
                transform.SetSiblingIndex(value._Index);
                 
                if (value._Data==null)
                {
                    _OnCellNameChanged?.Invoke(string.Empty);
                }
                else
                {
                    _OnCellNameChanged?.Invoke(value._Data.ToString());
                }

            }
        }
        /// <summary>
        /// ������ͷ����
        /// </summary>
        public HeaderBase _HeaderBase { get {
                if (!headerBase)
                {
                    headerBase = GetComponentInParent<HeaderBase>();
                }
                return headerBase;
            } }

        HeaderBase headerBase;

        RectTransform rectTransform;
        public RectTransform _RectTransform
        {
            get
            {
                if (!rectTransform)
                {
                    rectTransform = transform as RectTransform;
                }
                return rectTransform;
            } 
        }


        Mask parentMask;
        /// <summary>
        /// ����������
        /// </summary>
        protected Mask _ParentMask
        {
            get
            {
                if (!parentMask)
                {
                    parentMask = GetComponentInParent<Mask>();
                }
                return parentMask;
            } 
        }


        ToggleGroup  toggleGroup;
        public ToggleGroup _ToggleGroup
        {
            get
            {
                if (!toggleGroup&&  _HeaderBase)
                {
                    toggleGroup = _HeaderBase._ToggleGroup;
                }
                return  toggleGroup;
            } 
        }

        /// <summary>
        /// �Ƿ�����ʾ��Χ�߽���״̬�ı�ʱ����
        /// </summary>
        public event _IsInsideBoundaryChangeDelegate _IsInsideBoundaryChangedEvent;

        bool isInsideBoundary;
        /// <summary>
        /// �õ�Ԫ���Ƿ��ڱ߽��ڣ�ÿһ֡ˢ��һ��
        /// </summary>
        public bool _IsInsideBoundary
        {
            get
            {
                return isInsideBoundary;
            }
            set
            {
                if (isInsideBoundary == value) return;
                isInsideBoundary = value;
                _IsInsideBoundaryChangedEvent?.Invoke(this,value);
            }
        }

        /// <summary>
        /// �õ�Ԫ���Ƿ��ڱ߽���
        /// </summary>
        public abstract bool InsideBoundary();
         
        /// <summary>
        /// ���ݷ����仯ʱ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _CellData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_CellData == null) return;

            if (e.PropertyName == nameof(HeaderCellData._Data))
            {
                if (_CellData._Data==null)
                {
                    _OnCellNameChanged?.Invoke(string.Empty);
                }
                else
                {
                    _OnCellNameChanged?.Invoke(_CellData._Data.ToString());
                }
               
            }
            else if (e.PropertyName == nameof(HeaderCellData._Index))
            {
                transform.SetSiblingIndex(_CellData._Index);
            }
        }
        protected override void Awake()
        {
            base.Awake();
            this.group = _ToggleGroup;
        }
        protected override void Start()
        {
            base.Start();
            _DragButton.transform.SetSiblingIndex(transform.childCount);
            _DragButton._OnEndDragEvent -= _DragButton__OnEndDragEvent;
            _DragButton._OnEndDragEvent += _DragButton__OnEndDragEvent;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (cellData != null)
            {
                cellData.PropertyChanged -= _CellData_PropertyChanged;
            }
            if (_DragButton)
            {
                _DragButton._OnEndDragEvent -= _DragButton__OnEndDragEvent;
            }
        }

        /// <summary>
        /// ��ק��ť����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _DragButton__OnEndDragEvent(object sender, UnityEngine.EventSystems.PointerEventData e) {
            _HeaderBase._ResetCellContentSize();
        }

        private void Update()
        {
            _IsInsideBoundary = InsideBoundary();

        }

    }
}