(*
  ------------------------------------------------------------------------------
  MDX Эксперт, АС "Финансовый Анализ", НПО "Криста, 2004г.
  ------------------------------------------------------------------------------
  Диалоговая формочка для указания места сохранения отчета в каталоге.
  При попытке пользователя сохранить отчет в каталоге,
  появляется эта формочка в модельном режиме что бы пользователь мог указать,
  где именно в каталоге нужно сохранять.
  ------------------------------------------------------------------------------
*)
unit fmRepositoryPath;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, fmFlatPanel, fmRepositoryView, Buttons, fmSpeedButton,
  fmEdit, ImgList, ComCtrls;

type
  TfrmRepositoryPath = class(TForm)
    Label1: TLabel;
    Label2: TLabel;
    edFileName: TEdit;
    fpTreeViewContainer: TPanel;
    btCancel: TButton;
    btOK: TButton;
    Bevel1: TBevel;
    RepositoryIL: TImageList;
    procedure btCloseClick(Sender: TObject);
    procedure RepViewChange(Sender: TObject; Node: TTreeNode);

  private
    { Дерево для отображения структуры каталога }
    FRepView: TfmRepositoryView;
    FFileName: string;
    FRootDir: string;
    procedure SetFileName(Value: string);
  public
    { Запрос пути (вызов диалога) }
    function QueryPath(Root, Extension: String): String;
    property FileName: string  read FFileName write SetFileName ;
  end;

implementation

{$R *.DFM}

{
  Запрос пути сохранения. Этим методом нужно вызывать данный диалог.
  Root  - корень каталога
  Extension - расширение шаблонов
  IL  - глифы для дерева, где отображается структура
}
function TfrmRepositoryPath.QueryPath(Root, Extension: String): string;
begin
  result := '';
  FRootDir := Root;
  {Создаем и инициализируем просмотрщик дерева }
  if not Assigned(FRepView) then
  begin
    FRepView := TfmRepositoryView.Create(fpTreeViewContainer);
    FRepView.Align := alClient;
    FRepView.OnChange := RepViewChange;
  end;


  with FRepView do
    begin
      Images := RepositoryIL;
      DocumentsExtension := 'exd,doc,ppt,xls';
      FolderOnly := true;
      RootDirectory := Root;
      if Items.count > 0 then
      begin
        Items[0].Selected := true;
        Items[0].Expand(false);
      end;
      DragMode:=dmManual;
      edFileName.Text := FileName;
      repeat
        if ShowModal = mrOK then
          begin
            if edFileName.Text <> '' then
              begin
                FFileName := edFileName.Text;
                result :=  RootDirectory + edFileName.Text;

{                if Selected = nil then
                  result :=  RootDirectory + edFileName.Text
                else
                begin
                  result :=  GetTemplatePath(Selected);
                  if Selected = TopItem then
                    result := result + edFileName.Text;
                 end;                                  }
                  
                //result := result + edFileName.Text;
                if ExtractFileExt(result) = '' then
                  result := result + '.' + Extension;
              end;
          end
          else break;
      until result<>'';
      DragMode:=dmAutomatic;
    end;
end;

procedure TfrmRepositoryPath.SetFileName(Value: string);
begin
  FFileName := Value;
  edFilename.Text := Value;
end;

procedure TfrmRepositoryPath.RepViewChange(Sender: TObject; Node: TTreeNode);
begin
  if (Node <> nil) then
  begin
    if (Node <> FRepView.TopItem) then
      edFileName.Text := StringReplace(FRepView.GetTemplatePath(Node),
        FRootDir, '', [rfReplaceAll, rfIgnoreCase])
    else
      edFileName.Text := '';
  end;
end;

{закрытие диалога}
procedure TfrmRepositoryPath.btCloseClick(Sender: TObject);
begin
  close;
end;

end.
