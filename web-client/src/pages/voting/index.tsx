import React, {useCallback, useState} from "react"
import {
  Accordion,
  AccordionDetails,
  AccordionSummary as MuiAccordionSummary,
  Button,
  Container,
  Grid,
  Paper,
  Link as MaterialLink,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  withStyles,
  ButtonGroup,
  makeStyles,
  createStyles
} from "@material-ui/core"
import Link from 'next/link'
import ExpandMoreIcon from '@material-ui/icons/ExpandMore'
import Navbar from "../../components/Navbar"
import {FiberManualRecord, FileCopyOutlined, FormatListBulleted} from "@material-ui/icons"
import theme from "../../theme"
import {AuthProps, withAuth} from "../../lib/auth"
import Layout from "../../components/Layout"

const AccordionSummary = withStyles({
  root: {
    backgroundColor: 'rgba(0, 0, 0, .03)',
    borderBottom: '1px solid rgba(0, 0, 0, .125)',
    marginBottom: -1,
    minHeight: 56,
    '&$expanded': {
      minHeight: 56,
    },
  },
  content: {
    '&$expanded': {
      margin: '12px 0',
    },
  },
  expanded: {},
})(MuiAccordionSummary)

const useStyles = makeStyles(theme =>
  createStyles({
    fileListItem: {
      padding: theme.spacing(1)
    }
  })
)

interface VoteType {
  id: number,
  name: string
}

interface UserVote {
  voteId: number,
  voteTypeId: number
}

export const getServerSideProps = withAuth(async ({req}) => {
  return {
    props: {}
  }
}, {withRedirect: true})

const Voting: React.FC<AuthProps> = (props) => {

  const voteTypes = [
    {
      id: 1,
      name: "Vote for",
      color: theme.palette.success
    },
    {
      id: 2,
      name: "Vote against",
      color: theme.palette.success
    },
    {
      id: 3,
      name: "Abstain",
      color: theme.palette.success
    }
  ]

  const [questions, setQuestions] = useState([
    {
      id: 1,
      name: "Question 1",
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget.",
      files: [
        {
          id: 1,
          name: "Qwerty.pdf",
          link: "https://localhost:3000"
        },
        {
          id: 2,
          name: "Qwerty.docx",
          link: "https://localhost:3000"
        },
        {
          id: 3,
          name: "Qwerty.xml",
          link: "https://localhost:3000"
        }
      ],
      expanded: true
    },
    {
      id: 2,
      name: "Question 2",
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse malesuada lacus exsit amet blandit leo lobortis eget.",
      files: [
        {
          id: 4,
          name: "Qwerty.pdf",
          link: "https://localhost:3000"
        }
      ],
      expanded: true
    }
  ])

  const [answers, setAnswers] = useState<UserVote[]>([])

  const handleVoting = useCallback((voteId, voteTypeId) => {
    let userVote = answers.find(_ => _.voteId === voteId)

    if (userVote) {
      setAnswers(answers.map(_ => {
        if (_.voteId === voteId) {
          return {
            ..._,
            voteTypeId,
          }
        }
        return _
      }))
    } else {
      setAnswers(answers.concat({voteId, voteTypeId}))
    }
  }, [answers])

  const classes = useStyles()


  return (
    <Layout title={'Current voting'} user={props.data?.user}>

      <Container maxWidth={'xl'}>
        <Typography component={'div'}>
          <Navbar/>
          {
            questions.map((question, index) => (
              <Accordion variant={'outlined'} key={index} expanded={question.expanded}>
                <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
                  <Grid container justifyContent={'space-between'}>
                    <Grid item>
                      <Typography>{question.name}</Typography>
                    </Grid>
                    <Grid item>
                      {voteTypes.find(voteType => answers.find(_ => _.voteId === question.id)?.voteTypeId === voteType.id)?.name}
                    </Grid>
                  </Grid>

                </AccordionSummary>

                <AccordionDetails>
                  <Grid container direction={'column'}>
                    <Grid item>
                      <Typography>
                        {question.description}
                      </Typography>
                    </Grid>
                    <Grid item>
                      <ol>
                        {
                          question.files.map((file, index) => (
                            <li className={classes.fileListItem} key={index}>
                              <Grid container direction={'row'} spacing={1}>
                                <Grid item>
                                  <FileCopyOutlined/>
                                </Grid>
                                <Grid item>
                                  <Link href={file.link} passHref={true}>
                                    <MaterialLink color={'inherit'}
                                                  aria-label={'Download file'}>
                                      <Typography variant={'subtitle1'}>
                                        {file.name}
                                      </Typography>
                                    </MaterialLink>
                                  </Link>
                                </Grid>
                              </Grid>
                            </li>
                          ))
                        }
                      </ol>
                    </Grid>
                    <Grid item style={{marginInline: 'auto'}}>
                      <ButtonGroup>
                        {voteTypes.map((item, index) => (
                          <Button
                            key={index}
                            onClick={() => handleVoting(question.id, item.id)}
                          >
                            {item.name}
                          </Button>
                        ))}
                      </ButtonGroup>
                    </Grid>
                  </Grid>
                </AccordionDetails>
              </Accordion>
            ))
          }

          <Accordion variant={'outlined'}>
            <AccordionSummary expandIcon={<ExpandMoreIcon/>}>
              <Typography>Confirmation</Typography>
            </AccordionSummary>

            <AccordionDetails>
              <Container maxWidth={'xl'}>
                <Grid container direction={'column'} spacing={3}>
                  <Grid item>
                    <TableContainer
                      component={props => <Paper variant={'outlined'}>{props.children}</Paper>}>
                      <Table size={'small'}>
                        <TableHead>
                          <TableRow>
                            <TableCell>
                              Question
                            </TableCell>
                            <TableCell align={'right'}>
                              Your vote
                            </TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {answers.map((answer, index) => (
                            <TableRow key={index}>
                              <TableCell>
                                {questions.find(_ => _.id === answer.voteId)?.name ?? ""}
                              </TableCell>
                              <TableCell align={'right'}>
                                {voteTypes.find(_ => _.id === answer.voteTypeId)?.name ?? ""}
                              </TableCell>
                            </TableRow>)
                          )}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  </Grid>
                  <Grid item container spacing={1} justifyContent={'space-between'}>
                    <Grid item>
                      <Typography>
                        Authorize votes: {answers.length} (
                        <Link href={'/'} passHref={true}>
                          <MaterialLink>
                            details
                          </MaterialLink>
                        </Link>
                        )
                      </Typography>
                    </Grid>
                    <Grid item>
                      <Button variant={'contained'} color={'primary'}>Confirm and submit my
                        votes</Button>
                    </Grid>
                  </Grid>
                </Grid>
              </Container>
            </AccordionDetails>
          </Accordion>


        </Typography>
      </Container>
    </Layout>

  )
}

export default Voting
