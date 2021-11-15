using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace app_movie
{
    public partial class Inserimento : System.Web.UI.Page
    {
        private System.Data.SqlClient.SqlConnection SqlConnection = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBMovie"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            String Query = "Select gender.genereid as id,gender.descrizione as descrizione from dbo.genere gender";
            if (!Page.IsPostBack)
            {
                if (SqlConnection.State == System.Data.ConnectionState.Open)
                {
                    SqlConnection.Close();
                }
                SqlConnection.Open();
                SqlDataSource1.SelectCommand = Query;
                SqlDataSource1.ConnectionString = SqlConnection.ConnectionString.ToString();
                Generi.DataSource = SqlDataSource1;
                Generi.DataValueField = "id";
                Generi.DataTextField = "descrizione";
                Generi.DataBind();
                Generi.Items.Insert(0, new ListItem("Seleziona un genere di film","0"));

            }
        }

        protected void Anni_Load(object sender, EventArgs e)
        {
            ListItem ListItem = null;
            //recuperiamo gli anni e popoliamo la dropdownListControl
            //partendo dall'anno de primo film della storia all'anno attuale recuperato
            //dalla data corrente
            //da fare solo al primo caricamento
            if (!Page.IsPostBack)
            {
                ListItem = new ListItem();
                ListItem.Text = "Seleziona anno di produzione";
                ListItem.Value = "0";
                Anni.Items.Insert(0, ListItem);
                for (Int32 i = 1895; i <= DateTime.Now.Year; i++)
                {
                    ListItem = new ListItem();
                    ListItem.Text = i.ToString();
                    ListItem.Value = i.ToString();
                    Anni.Items.Add(ListItem);
                }
            }


        }

        protected Boolean Processa()
        {
            if (this.SqlConnection.State == System.Data.ConnectionState.Open)
            {
                this.SqlConnection.Close();
            }
            this.SqlConnection.Open();
            Boolean RollBack = false;

            SqlTransaction SqlTransaction = this.SqlConnection.BeginTransaction();
            long id = Inserimento_Film(SqlTransaction);
            if (id != 0L)
            {
                Boolean Result = InserisciAssocGeneriFilm(id, Int32.Parse(Generi.SelectedValue), SqlTransaction);
                if (!Result)
                {
                    RollBack = true;
                }
                else
                {
                    Result = UploadFile();
                    if (!Result)
                    {
                        RollBack = true;
                    }
                }
            }
            else
            {
                RollBack = true;
            }

            if (RollBack)
            {
                SqlTransaction.Rollback();
            }
            else
            {
                SqlTransaction.Commit();
            }
            SqlConnection.Close();

            return !RollBack;
        }

        protected long Inserimento_Film(SqlTransaction SqlTransaction)
        {

            long IdFilm = 0L;
            String QueryInsertFilm = "INSERT INTO dbo.film (filmid,titolo,anno,copertina) VALUES(@Id,@Titolo,@Anno,@Copertina)";
            SqlCommand SqlCommand = null;
            SqlParameter Parameter0 = null;
            SqlParameter Parameter1 = null;
            SqlParameter Parameter2 = null;
            SqlParameter Parameter3 = null;
            if (Titolo.Text != null && !"".Equals(Titolo.Text)
                && Generi.SelectedValue != "0" && Anni.SelectedValue != "0" && Copertina.FileName != null)
            {
                IdFilm = GeneraIdFilm(SqlTransaction);
                if (IdFilm != 0L)
                {
                    SqlCommand = this.SqlConnection.CreateCommand();
                    SqlCommand.Transaction = SqlTransaction;
                    SqlCommand.CommandText = QueryInsertFilm;
                    Parameter0 = new SqlParameter("@Id", System.Data.SqlDbType.Int, 0);
                    Parameter0.Value = (int)IdFilm;
                    Parameter1 = new SqlParameter("@Titolo", System.Data.SqlDbType.NVarChar, 100);
                    Parameter1.Value = Titolo.Text;
                    Parameter2 = new SqlParameter("@Anno", System.Data.SqlDbType.Int, 0);
                    Parameter2.Value = Anni.SelectedValue;
                    Parameter3 = new SqlParameter("@Copertina", System.Data.SqlDbType.NVarChar, 200);
                    Parameter3.Value = Copertina.PostedFile.FileName;
                    SqlCommand.Parameters.Add(Parameter0);
                    SqlCommand.Parameters.Add(Parameter1);
                    SqlCommand.Parameters.Add(Parameter2);
                    SqlCommand.Parameters.Add(Parameter3);
                    SqlCommand.Prepare();
                    Int32 Result = SqlCommand.ExecuteNonQuery();
                    if (Result == 1)
                    {
                        return IdFilm;
                    }
                }
                else
                {
                    throw new Exception("Errore nella generazione identificativo film");
                }

            }
            return 0L;
        }


        protected long GeneraIdFilm(SqlTransaction SqlTransaction)
        {
            long id = 0L;
            String Query = "SELECT NEXT VALUE for dbo.id_film_sequence";
            SqlCommand SqlCommand = this.SqlConnection.CreateCommand();
            SqlCommand.Transaction = SqlTransaction;
            SqlCommand.CommandText = Query;
            SqlCommand.Prepare();
            SqlDataReader dr = SqlCommand.ExecuteReader();
            while (dr.Read())
            {
                var Obj = dr.GetValue(0);
                id = long.Parse(Obj.ToString());
                break;
            }
            dr.Close();
            return id;
        }

        protected Boolean InserisciAssocGeneriFilm(long FkFilm, Int32 FkGenere, SqlTransaction SqlTransaction)
        {
            Boolean Insert = false;
            String QueryInserAssocFilmGeneri = "INSERT into dbo.ass_film_genere (fk_film,fk_genere) VALUES(@FkFilm,@FkGenere)";
            SqlCommand SqlCommand = this.SqlConnection.CreateCommand();
            SqlCommand.Transaction = SqlTransaction;
            SqlCommand.CommandText = QueryInserAssocFilmGeneri;
            SqlParameter SqlParameter = new SqlParameter("@FkFilm", System.Data.SqlDbType.Int, 0);
            SqlParameter.Value = FkFilm;
            SqlParameter SqlParameter2 = new SqlParameter("@FkGenere", System.Data.SqlDbType.Int, 0);
            SqlParameter2.Value = FkGenere;
            SqlCommand.Parameters.Add(SqlParameter);
            SqlCommand.Parameters.Add(SqlParameter2);
            SqlCommand.Prepare();
            Int32 result = SqlCommand.ExecuteNonQuery();
            if (result == 1)
            {
                Insert = true;
            }
            return Insert;
        }

        protected Boolean UploadFile()
        {
            String Destination = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("upload_path");
            if (Destination != null && !"".Equals(Destination))
            {
                if (Copertina.PostedFile.ContentLength == 0)
                {
                    return false;
                }
                if (Copertina.PostedFile.ContentLength > 0 && Copertina.PostedFile.FileName != null)
                {
                    Copertina.PostedFile.SaveAs(Server.MapPath(Destination) + Copertina.PostedFile.FileName);
                    return true;
                }
            }
            return false;
        }

        protected void Inserisci_Click(object sender, EventArgs e)
        {
            Boolean result = Processa();
        }

        protected void Reset_Click(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                if (Titolo.Text != "")
                {
                    Titolo.Text = "";
                }
                if (Copertina.Attributes.Count != 0)
                {
                    Copertina.Attributes.Clear();
                }
            }
        }
    }
}