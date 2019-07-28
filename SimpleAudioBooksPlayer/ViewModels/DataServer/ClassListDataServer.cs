using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.Extensions;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class ClassListDataServer : IDataServer<ClassItemDTO, ClassItemDTO>
    {
        public static readonly ClassListDataServer Current = new ClassListDataServer();

        public static ClassItemDTO Unspecified_ClassItem { get; private set; }
        public static ClassItemDTO All_ClassItem { get; private set; }

        private readonly ClassListDataService _service = ClassListDataService.Current;

        private ClassListDataServer()
        {
            _service.DataAdded += Service_DataAdded;
        }

        public event EventHandler<IEnumerable<ClassItemDTO>> DataLoaded;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataAdded;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataRemoved;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataUpdated;

        public bool IsInit { get; private set; }
        public ObservableCollection<ClassItemDTO> Data { get; } = new ObservableCollection<ClassItemDTO>();

        public async Task Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            this.LogByObject("初始化分类服务器");
            var dataSource = await _service.GetData();
            foreach (var item in dataSource)
                Data.Add(new ClassItemDTO(item));

            if (Data.All(c => c.Index != -1))
            {
                this.LogByObject("添加 ‘未指定’ 分类");
                await _service.Add(-1, StringResources.DefaultValues.GetString("Unspecified_ClassItem"));
            }

            if (Data.All(c => c.Index != 1))
            {
                this.LogByObject("添加 ‘全部’ 分类");
                await Add(StringResources.DefaultValues.GetString("All_ClassItem"));
            }

            Unspecified_ClassItem = Data.First(c => c.Index == -1);
            All_ClassItem = Data.First(c => c.Index == 1);

            DataLoaded?.Invoke(this, Data.ToList());
        }

        public async Task Remove(ClassItemDTO itemDto)
        {
            if (!Data.Contains(itemDto))
                return;

            this.LogByObject("正在删除分类");
            Data.Remove(itemDto);
            await _service.Remove(itemDto.Index);

            DataRemoved?.Invoke(this, new[] {itemDto});
        }

        public async Task Rename(ClassItemDTO itemDto, string newName)
        {
            if (!Data.Contains(itemDto))
                return;

            this.LogByObject("正在重命名分类");
            itemDto.Name = newName;
            await _service.Rename(itemDto.Index, newName);
            DataUpdated?.Invoke(this, new[] {itemDto});
        }

        public async Task SetBackgroundColor(ClassItemDTO itemDto, Color backgroundColor)
        {
            if (!Data.Contains(itemDto))
                return;

            this.LogByObject("正在设置背景颜色");
            itemDto.BackgroundColor = backgroundColor;
            await _service.SetBackgroundColor(itemDto.Index, backgroundColor.ToArgbString());
            DataUpdated.Invoke(this, new[] {itemDto});
        }

        // 因为要获取id号，所以只能用事件回调来添加项目
        public Task Add(string name) => _service.Add(name);

        private void Service_DataAdded(object sender, IEnumerable<ClassItem> e)
        {
            this.LogByObject("正在添加分类");
            var source = e.Select(c => new ClassItemDTO(c)).ToList();
            foreach (var dto in source)
                if (Data.Any() && dto.Index == -1)
                    Data.Insert(0, dto);
                else
                    Data.Add(dto);

            DataAdded?.Invoke(this, source);
        }
    }
}