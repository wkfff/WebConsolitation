(*
  ------------------------------------------------------------------------------
  MDX �������, �� "���������� ������", ��� "������, 2004�.
  ------------------------------------------------------------------------------
  ���������� �������� ��� �������� ����� ���������� ������ � ��������.
  ��� ������� ������������ ��������� ����� � ��������,
  ���������� ��� �������� � ��������� ������ ��� �� ������������ ��� �������,
  ��� ������ � �������� ����� ���������.
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
    { ������ ��� ����������� ��������� �������� }
    FRepView: TfmRepositoryView;
    FFileName: string;
    FRootDir: string;
    procedure SetFileName(Value: string);
  public
    { ������ ���� (����� �������) }
    function QueryPath(Root, Extension: String): String;
    property FileName: string  read FFileName write SetFileName ;
  end;

implementation

{$R *.DFM}

{
  ������ ���� ����������. ���� ������� ����� �������� ������ ������.
  Root  - ������ ��������
  Extension - ���������� ��������
  IL  - ����� ��� ������, ��� ������������ ���������
}
function TfrmRepositoryPath.QueryPath(Root, Extension: String): string;
begin
  result := '';
  FRootDir := Root;
  {������� � �������������� ����������� ������ }
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

{�������� �������}
procedure TfrmRepositoryPath.btCloseClick(Sender: TObject);
begin
  close;
end;

end.
