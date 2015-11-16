﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Acceleratio.SPDG.Generator;

namespace Acceleratio.SPDG.UI
{
    public partial class frm02UsersGroups : frmWizardMaster
    {
        public frm02UsersGroups()
        {
            InitializeComponent();

            base.lblTitle.Text = "Users && Groups";
            base.lblDescription.Text = "Create user and group accounts in your Active Directory";

            btnNext.Click += btnNext_Click;
            btnBack.Click += btnBack_Click;

            this.Text = Common.APP_TITLE;
            ucSteps1.showStep(2);

            loadData();
            chkGenerateUsers_CheckedChanged(null, EventArgs.Empty);
            cboDomains.SelectedIndexChanged += cboDomains_SelectedIndexChanged;
        }
        public new ServerGeneratorDefinition WorkingDefinition
        {
            get { return (ServerGeneratorDefinition) base.WorkingDefinition; }
        }


        void btnBack_Click(object sender, EventArgs e)
        {
            preventCloseMessage = true;
            RootForm.MovePrevious(this);
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            preventCloseMessage = true;
            RootForm.MoveNext(this);
        }

        public override void loadData()
        {
            this.Show();
            this.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            List<string> domains = AD.GetDomainList();

            foreach (string domain in domains)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = domain;
                item.Value = domain;
                cboDomains.Items.Add(item);
            }
            cboDomains.Text = domains[0];

            List<string> subdomains = AD.GetDomainList2();

            foreach (string domain in subdomains)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = domain;
                item.Value = domain;
                cboDomains.Items.Add(item);
            }

            chkGenerateUsers.Checked = WorkingDefinition.GenerateUsersAndSecurityGroupsActiveInDirectory;
            trackNumberOfUsers.Value = WorkingDefinition.NumberOfUsersToCreate;
            trackNumberOfSecGroups.Value = WorkingDefinition.NumberOfSecurityGroupsToCreate;
            if (!string.IsNullOrEmpty(WorkingDefinition.ADDomainName))
            {
                cboDomains.Text = WorkingDefinition.ADDomainName;
            }
            
            cboOrganizationalUnit.Text = WorkingDefinition.ADOrganizationalUnit;

            this.Show();
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        public override bool saveData()
        {
            WorkingDefinition.GenerateUsersAndSecurityGroupsActiveInDirectory = chkGenerateUsers.Checked;
            WorkingDefinition.NumberOfUsersToCreate = trackNumberOfUsers.Value;
            WorkingDefinition.NumberOfSecurityGroupsToCreate = trackNumberOfSecGroups.Value;
            WorkingDefinition.ADDomainName = cboDomains.Text;
            WorkingDefinition.ADOrganizationalUnit = cboOrganizationalUnit.Text;

            return true;
        }

        private void chkGenerateUsers_CheckedChanged(object sender, EventArgs e)
        {
            if( chkGenerateUsers.Checked)
            {
                groupBox1.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = false;
                WorkingDefinition.NumberOfUsersToCreate = 0;
                WorkingDefinition.NumberOfSecurityGroupsToCreate = 0;
            }
        }

        void cboDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDomains.SelectedItem == null)
            {
                return;
            }

            fillOUs();
        }


        private void cboDomains_Leave(object sender, EventArgs e)
        {
            if(cboDomains.Text == string.Empty)
            {
                return;
            }

            fillOUs();
        }

        private void fillOUs()
        {
            this.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            List<string> ous = AD.ListOU(cboDomains.Text);
            cboOrganizationalUnit.Items.Clear();
            foreach (string ou in ous)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = ou;
                item.Value = ou;
                cboOrganizationalUnit.Items.Add(item);
            }

            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void trackNumberOfUsers_ValueChanged(object sender, EventArgs e)
        {
            lblNumUsers.Text = trackNumberOfUsers.Value.ToString();
        }

        private void trackNumberOfSecGroups_ValueChanged(object sender, EventArgs e)
        {
            lblGroups.Text = trackNumberOfSecGroups.Value.ToString();
        }
    }
}
